namespace Unity.Behavior.Demo
{
    /// <summary>
    /// Character state channel dependency injection.
    /// </summary>
    public interface ICharacterStateChannelModifier
    {
        public CharacterStateEventChannel StateChannel { get; set; }
    }
}
