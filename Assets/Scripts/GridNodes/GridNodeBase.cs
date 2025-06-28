
using Grid;

public interface GridNodeBase
{
    GridNodeType nodeType { get; }
    public void OnHeroEnter(Hero hero) { return; }
    public void OnHeroExit(Hero hero) { return; }
    public void OnStartHeroMove(Hero hero) { return; }
    public void OnStartBuild() { return; }
}
