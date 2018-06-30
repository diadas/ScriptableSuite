namespace ScriptableSuite.Variables
{
    public interface IScriptableVariableListener<T>
    {
        void OnChange(IScriptableVariable<T> variable);
    }
}