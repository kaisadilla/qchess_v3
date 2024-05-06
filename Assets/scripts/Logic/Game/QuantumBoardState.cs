#nullable enable

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public class QuantumBoardState {
    private readonly List<ClassicBoardState> _classicStates;
    private readonly ChessGame _game;

    /// <summary>
    /// Represents the total amount of classic states contained within this
    /// quantum one. Since a classic state object may represent more than one
    /// identical classic state, this number cannot be obtained by simply
    /// counting the amount of items in the classic states' list.
    /// </summary>
    private BigInteger _totalClassicStates;

    public IReadOnlyList<ClassicBoardState> ClassicStates => _classicStates;

    public QuantumBoardState (ChessGame game, ClassicBoardState initialState) {
        _game = game;
        _classicStates = new() { initialState };
        _totalClassicStates = initialState.Weight;
    }

    public QuantumBoardState (ChessGame game, List<ClassicBoardState> classicStates) {
        _game = game;
        _classicStates = classicStates;
        CalculateClassicStateCount();
    }

    public QuantumBoardState (ChessGame game, QuantumBoardState original) {
        _game = game;
        _classicStates = new(original._classicStates); // TODO: we should probably deep copy classic states.
        // we are cloning the state, so this number cannot be different.
        _totalClassicStates = original._totalClassicStates;
    }

    public List<RealPiece> GetPiecesAtPos (Vector2Int pos) {
        // We'll iterate over all the classic states in this quantum state,
        // checking the piece in the given cell in each.
        // Whenever we find one piece, we increase its counter by 1.
        // Keys in this dictionary represent different piece ids found in the
        // cell we are looking at, while values represent the amount of times
        // that specific id was found there.
        Dictionary<int, long> appearances = new();

        foreach (var state in _classicStates) {
            if (state.IsAnyPieceAt(pos) == false) continue;

            // if we are here, then there's a piece in this state and cell.
            int id = state[pos];

            if (appearances.ContainsKey(id)) {
                appearances[id] += state.Weight;
            }
            else {
                appearances[id] = state.Weight;
            }
        }

        List<RealPiece> pieces = new();

        foreach (var kv in appearances) {
            var pieceId = kv.Key;

            double presence = kv.Value / (double)_totalClassicStates;
            pieces.Add(new(_game, pieceId, pos, presence));
        }

        return pieces;
    }

    public List<RealPiece> GetCapturedPieces () {
        Dictionary<int, long> appearances = new();

        foreach (var state in _classicStates) {
            foreach (var capturedPieceId in state.CapturedPieces) {
                if (appearances.ContainsKey(capturedPieceId)) {
                    appearances[capturedPieceId] += state.Weight;
                }
                else {
                    appearances[capturedPieceId] = state.Weight;
                }
            }
        }

        List<RealPiece> pieces = new();

        foreach (var kv in appearances) {
            var pieceId = kv.Key;

            double presence = kv.Value / (double)_totalClassicStates;
            pieces.Add(RealPiece.CreateCapturedPiece(_game, pieceId, presence));
        }

        return pieces;
    }

    public bool IsCellEmpty (Vector2Int pos) {
        foreach (var state in _classicStates) {
            if (state.IsAnyPieceAt(pos)) return false;
        }

        return true;
    }

    /// <summary>
    /// Makes a classic move on the board. A classic move does not alter
    /// the total amount of classic states that compose the quantum state.
    /// Classic states in which the move would not be legal don't execute
    /// the move.
    /// A move is considered classic when it only has one destination target,
    /// regardless of whether the piece involved is a classic or quantum piece,
    /// or whether the piece will split into two due to quantum entanglement.
    /// </summary>
    /// <param name="move">The description of the move to make.</param>
    public void MakeClassicMove (ClassicMove move) {
        if (move.MeasuresOrigin) {
            MeasureCell(move.Origin);
        }

        // this move causes its target cell to be measured. The cell must be
        // measured before the moved piece moves into it.
        if (move.MeasuresTarget) {
            MeasureCell(move.Target);
        }

        foreach (var state in _classicStates) {
            state.MakeMoveIfAble(move.PieceId, move.Origin, move.Target);
        }

        OptimizeClassicStateList(_classicStates);
    }

    public void MakeQuantumMove (QuantumMove move) {
        List<ClassicBoardState> newStates = new();

        Stopwatch s = Stopwatch.StartNew();

        foreach (var state in _classicStates) {
            var clone = state.Clone();

            state.MakeMoveIfAble(move.PieceId, move.Origin, move.Targets[0]);
            clone.MakeMoveIfAble(move.PieceId, move.Origin, move.Targets[1]);

            newStates.Add(clone);
        }

        s.Stop();
        UnityEngine.Debug.Log(
            $"Creating new boards: {s.ElapsedMilliseconds} ms."
        );

        var allNewStates = _classicStates.Concat(newStates);
        OptimizeClassicStateList(allNewStates);
    }

    /// <summary>
    /// Optimizes the classic state list to make it as small as possible, 
    /// merging redundant classic states in the list together, reducing the
    /// 'Weight' value of the states when they can be divided, etc.
    /// 
    /// This method takes an argument with the list of states to optimimze, but
    /// the result will be stored automatically in the '_classicStates' field.
    /// 
    /// This method will automatically update '_totalClassicStates'.
    /// </summary>
    /// <param name="states">The list of classic board states to optimize.</param>
    private void OptimizeClassicStateList (IEnumerable<ClassicBoardState> states) {
        Stopwatch s = Stopwatch.StartNew();

        List<ClassicBoardState> updatedStates = new();
        List<long> weights = new();
        bool weightOneFound = false; // true when a state with a Weight of '1' is found.

        foreach (var stateToAdd in states) {
            bool identicalBoardFound = false;

            foreach (var consolidatedState in updatedStates) {
                if (consolidatedState.IsBoardIdentical(stateToAdd)) {
                    consolidatedState.Weight += stateToAdd.Weight; // weights can be different, so can't multiply by 2.
                    identicalBoardFound = true;
                    break;
                }
            }

            if (identicalBoardFound == false) {
                updatedStates.Add(stateToAdd);
            }
        }

        foreach (var state in updatedStates) {
            weights.Add(state.Weight);

            // weights cannot be simplified, so we should count no further.
            if (state.Weight == 1) {
                weightOneFound = true;
                break;
            }
        }

        // if weights can potentially be simplified
        if (weightOneFound == false) {
            // find the greatest common denominator
            long gcd = Utils.GCD(weights.ToArray());

            // if its higher than 1, divide all states by the gcd.
            if (gcd > 1) {
                foreach (var state in updatedStates) {
                    state.Weight /= gcd;
                }

                UnityEngine.Debug.Log($"Board weights divided by {gcd}.");
            }
        }

        double timeToCreateBoardCollection = s.ElapsedMilliseconds;

        _classicStates.Clear();
        _classicStates.AddRange(updatedStates);

        CalculateClassicStateCount();

        s.Stop();
        UnityEngine.Debug.Log(
            $"{_classicStates.Count} boards ({_totalClassicStates} states) " +
            $"- optimized in {timeToCreateBoardCollection} ms"
        );
    }

    private void CalculateClassicStateCount () {
        _totalClassicStates = 0;

        foreach (var state in _classicStates) {
            _totalClassicStates += state.Weight;
        }
    }

    private void MeasureCell (Vector2Int cell) {
        Stopwatch s = Stopwatch.StartNew();

        // we get a random board, taking their weight into account.
        int boardIndex = GetRandomBoardIndex();
        var board = _classicStates[boardIndex];
        // the piece that is at that cell in the chosen board, or null
        // if there isn't any piece there in this board.
        int? pieceIdAtCell = board.GetPieceAt(cell);

        // the boards that will remain active after this measure.
        List<ClassicBoardState> survivingStates = new();
        foreach (var state in _classicStates) {
            // only boards that have the same piece (or lack of piece)
            // in the target cell as our reference board will survive.
            if (state.GetPieceAt(cell) == pieceIdAtCell) {
                survivingStates.Add(state);
            }
        }

        _classicStates.Clear();
        _classicStates.AddRange(survivingStates);

        s.Stop();
        UnityEngine.Debug.Log(
            $"Measurement taken for cell {cell} in " +
            $"{s.ElapsedMilliseconds} ms."
        );
    }

    private int GetRandomBoardIndex () {
        //long rng = Utils.RandomLong(0, (long)_totalClassicStates);
        int rng = Random.Range(0, (int)_totalClassicStates); // TODO: we need a long rng.

        long acc = 0;
        int selectedIndex = -1;

        for (int i = 0; i < _classicStates.Count; i++) {
            var state = _classicStates[i];
            acc += state.Weight;

            if (acc > rng) {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == -1) {
            throw new System.Exception("No index could be selected!");
        }

        return selectedIndex;
    }
}
