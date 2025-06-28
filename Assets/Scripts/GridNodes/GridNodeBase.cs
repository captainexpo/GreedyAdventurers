
using Grid;

public interface GridNodeBase
{
    GridNodeType nodeType { get; }
    void OnHeroEnter(Hero hero);
    void OnHeroExit(Hero hero);

}
