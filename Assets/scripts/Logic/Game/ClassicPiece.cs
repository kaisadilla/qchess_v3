#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using uint_t = System.UInt16;

public struct ClassicPiece {
    private const int CURSOR_PLAYER_ID = 0;
    private const int CURSOR_TYPE = 1;
    private const int CURSOR_ID = 4;

    private const uint_t BITS_PLAYER_ID = 0b1;
    private const uint_t BITS_TYPE = 0b111;
    private const uint_t BITS_ID = 0b1111_1111_1111;

    private const uint_t MASK_PLAYER_ID = BITS_PLAYER_ID << CURSOR_PLAYER_ID;
    private const uint_t MASK_TYPE = BITS_TYPE << CURSOR_TYPE;
    private const uint_t MASK_ID = BITS_ID << CURSOR_ID;

    private uint_t _data;

    /// <summary>
    /// The id of the player who owns this piece.
    /// </summary>
    public int PlayerId {
        readonly get {
            return (_data & MASK_PLAYER_ID) >> CURSOR_PLAYER_ID;
        }
        set {
            unchecked {
                uint_t newVal = (uint_t)(value & BITS_PLAYER_ID);

                _data &= (uint_t)~MASK_PLAYER_ID;
                _data |= (uint_t)(newVal << CURSOR_PLAYER_ID);
            }
        }
    }

    /// <summary>
    /// This piece's type.
    /// </summary>
    public PieceType Type {
        readonly get {
            return (PieceType)((_data & MASK_TYPE) >> CURSOR_TYPE);
        }
        set {
            unchecked {
                uint_t newVal = (uint_t)((uint_t)value & BITS_TYPE);

                _data &= (uint_t)~MASK_TYPE;
                _data |= (uint_t)(newVal << CURSOR_TYPE);
            }
        }
    }

    /// <summary>
    /// This piece's id.
    /// </summary>
    public int Id {
        readonly get {
            return (_data & MASK_ID) >> CURSOR_ID;
        }
        set {
            unchecked {
                uint_t newVal = (uint_t)(value & BITS_ID);

                _data &= (uint_t)~MASK_ID;
                _data |= (uint_t)(newVal << CURSOR_ID);
            }
        }
    }

    public readonly string PlayerName => PlayerId == 0 ? "White" : "Black";

    public ClassicPiece (int playerId, int id, PieceType type) {
        _data = 0;
        PlayerId = playerId;
        Id = id;
        Type = type;
    }

    public readonly string ValueToString () {
        return Convert.ToString(_data, 2).PadLeft(16, '0');
    }

    public override readonly string ToString () {
        return $"{PlayerId}:{Id}/{Type}";
    }
}
