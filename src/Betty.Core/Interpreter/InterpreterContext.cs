namespace Betty.Core.Interpreter
{
    public enum ControlFlowState
    {
        Normal,
        Return,
        Break,
        Continue
    }

    public class InterpreterContext
    {
        public ControlFlowState FlowState { get; set; } = ControlFlowState.Normal;
        public Value LastReturnValue { get; set; } = Value.None();
        public int LoopDepth { get; set; } = 0;

        // Method to enter a new loop, increasing loop depth
        public void EnterLoop() => LoopDepth++;

        // Method to exit a loop, decreasing loop depth
        public void ExitLoop()
        {
            if (LoopDepth > 0)
            {
                LoopDepth--;
            }
            else
            {
                // If not in a loop, throw an exception
                throw new InvalidOperationException("Attempted to exit loop when not in a loop.");
            }
        }

        // Check if currently inside a loop
        public bool IsInLoop => LoopDepth > 0;
    }
}