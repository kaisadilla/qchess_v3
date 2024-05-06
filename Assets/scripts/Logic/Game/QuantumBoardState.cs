#nullable enable

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using UnityEngine;


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
        _totalClassicStates = initialState.Multiplier;
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
                appearances[id] += state.Multiplier;
            }
            else {
                appearances[id] = state.Multiplier;
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
        foreach (var state in _classicStates) {
            state.MakeMoveIfAble(move.PieceId, move.Origin, move.Target);
        }

        UnityEngine.Debug.Log(
            $"[{_classicStates.Count} boards (representing {_totalClassicStates} " +
            $"states)]."
        );
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

        var allNewStates = _classicStates.Concat(newStates);

        double timeToCloneNewBoards = s.ElapsedMilliseconds;
        s.Restart();

        List<ClassicBoardState> updatedStates = new();
        foreach (var stateToAdd in allNewStates) {
            bool identicalBoardFound = false;

            foreach (var consolidatedState in updatedStates) {
                if (consolidatedState.IsBoardIdentical(stateToAdd)) {
                    consolidatedState.Multiplier += stateToAdd.Multiplier; // technically it'll always be multiplying by 2.
                    identicalBoardFound = true;
                    break;
                }
            }
            
            if (identicalBoardFound == false) {
                updatedStates.Add(stateToAdd);
            }
        }

        double timeToCreateBoardCollection = s.ElapsedMilliseconds;

        _classicStates.Clear();
        _classicStates.AddRange(updatedStates);
        CalculateClassicStateCount();

        s.Stop();
        UnityEngine.Debug.Log(
            $"[{_classicStates.Count} boards (representing {_totalClassicStates} " + 
            $"states)] - Move: {timeToCloneNewBoards} ms, " + 
            $"Generate board list: {timeToCreateBoardCollection} ms."
        );
    }

    private void CalculateClassicStateCount () {
        _totalClassicStates = 0;
        foreach (var state in _classicStates) {
            _totalClassicStates += state.Multiplier;
        }

    }
}
