public interface IRule<T>
{
    bool IsAllowed(T subject);
}
public class CanResolveQuantumRule : IRule<(GamePiece piece, EPlayer player)>
{
    readonly QuantumLinker _linker;
    public CanResolveQuantumRule(QuantumLinker linker, BoardState state)
    {
        _linker = linker;
    }
    public bool IsAllowed((GamePiece piece, EPlayer player) pair)
    {
        return
            pair.piece != null &&
            pair.piece.player == pair.player &&
            pair.piece is QuantumGamePiece &&
            _linker.IsResolvable(pair.piece as QuantumGamePiece);
    }
}
