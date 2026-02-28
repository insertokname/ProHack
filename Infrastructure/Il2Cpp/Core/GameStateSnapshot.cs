using Domain;

namespace Infrastructure.Il2Cpp.Core;

/// <summary>
/// Immutable, allocation-free snapshot of all game state values read in a single
/// <see cref="FridaChannel"/> round-trip.
/// </summary>
/// <remarks>
/// Capture once per game loop tick with <see cref="PROIl2CppManager.ReadAll"/>
/// and pass the value around rather than issuing individual reads.
/// Every property is derived from the raw values supplied by the agent —
/// no further memory access occurs after construction.
/// </remarks>
/// <param name="SelectedMenu">
///   The current battle-menu state, translated from the raw int via
///   <see cref="SelectedMenuTools.FromMemory"/>.
/// </param>
/// <param name="CurrentEncounterId">
///   The Pokémon ID of the active wild encounter, or <c>0</c> when none.
///   Maps to <c>DSSock.OtherPoke</c> in the game binary.
/// </param>
/// <param name="IsBattling">
///   <see langword="true"/> when the player is inside a battle.
///   Derived from <c>DSSock.ply</c> (non-zero = battling).
/// </param>
/// <param name="ShinyForm">
///   Raw shiny-form flag from the game (<c>0</c> = not shiny).
///   Read from <c>DSSock instance + 0x7E0</c>.
/// </param>
/// <param name="EventForm">
///   Raw event-form flag from the game (<c>0</c> = not an event Pokémon).
///   Read from <c>DSSock instance + 0x7E4</c>.
/// </param>
public readonly record struct GameStateSnapshot(
    SelectedMenuEnum SelectedMenu,
    int              CurrentEncounterId,
    bool             IsBattling,
    int              ShinyForm,
    int              EventForm,
    float            PlayerX,
    float            PlayerY)
{
    /// <summary>
    /// <see langword="true"/> when the current encounter is shiny (<see cref="ShinyForm"/> ≠ 0)
    /// or is an event Pokémon (<see cref="EventForm"/> ≠ 0).
    /// </summary>
    public bool IsSpecial => ShinyForm != 0 || EventForm != 0;

    /// <summary>
    /// Creates a <see cref="GameStateSnapshot"/> from the raw integer values returned
    /// by the in-process IL2CPP agent.
    /// </summary>
    /// <param name="selectedMenuRaw">Raw SelectedMenu int from the agent.</param>
    /// <param name="currentEncounterId">Raw CurrentEncounterId int.</param>
    /// <param name="isBattlingRaw">Raw IsBattling int (0 = false).</param>
    /// <param name="shinyForm">Raw ShinyForm int.</param>
    /// <param name="eventForm">Raw EventForm int.</param>
    internal static GameStateSnapshot FromRaw(
        int   selectedMenuRaw,
        int   currentEncounterId,
        int   isBattlingRaw,
        int   shinyForm,
        int   eventForm,
        float playerX,
        float playerY)
        => new(
            SelectedMenu:       SelectedMenuTools.FromMemory(selectedMenuRaw),
            CurrentEncounterId: currentEncounterId,
            IsBattling:         isBattlingRaw != 0,
            ShinyForm:          shinyForm,
            EventForm:          eventForm,
            PlayerX:            playerX,
            PlayerY:            playerY);

    /// <inheritdoc/>
    public override string ToString()
        => $"[Menu={SelectedMenu} Battling={IsBattling} EncId={CurrentEncounterId} Shiny={ShinyForm} Event={EventForm} Pos=({PlayerX},{PlayerY})]";
}
