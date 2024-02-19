/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

namespace AstralCandle.Entity{
    public enum EntityERR{
        SUCCESS,
        INVALID_CALL,
        NOT_PERMITTED, // Action is not permitted. (Perhaps needs ownership of entity)
        NOT_IN_RANGE, // Entity is not within range to perform action
        IS_FRIENDLY, // Entity is on the same team
        IS_IMMORTAL, // Entity is unable to be damaged
        NO_OCCUPANTS, // Entity has no occupants
        MAX_OCCUPANTS, // Entity is at max occupancy
        UNDER_COOLDOWN, // Entity is under cooldown for action
        IS_LOCKED, // Entity is stunned
    }
}