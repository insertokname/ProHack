namespace Domain
{
    // 0 = outside of fight
    // 41 = None selected
    // 41 = fight menu
    // 42 = Can't interact
    // 46 = items menu
    // 55 = pokemon menu
    // 47 = Fight (superboss)
    // 61 = Pokemon (superboss)
    // 52 = Item (superboss)
    public enum SelectedMenuEnum
    {
        OutSideOfFight,
        FightOrNoneMenu,
        CantInteract,
        ItemsMenu,
        PokemonMenu,
    }

    public class SelectedMenuTools
    {
        public static SelectedMenuEnum FromMemory(int x)
        {
            if (x == 0)
                return SelectedMenuEnum.OutSideOfFight;
            else if (x == 41)
                return SelectedMenuEnum.FightOrNoneMenu;
            else if (x == 42)
                return SelectedMenuEnum.CantInteract;
            else if (x == 46)
                return SelectedMenuEnum.ItemsMenu;
            else if (x == 55)
                return SelectedMenuEnum.PokemonMenu;
            throw new ArgumentException($"Invalid state of {nameof(SelectedMenuEnum)}! Got invalid value: {x}!");
        }
    }
}