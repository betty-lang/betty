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

        public void EnterLoop() => LoopDepth++;

        public void ExitLoop()
        {
            if (LoopDepth > 0)
            {
                LoopDepth--;
            }
        }

        public bool IsInLoop => LoopDepth > 0;

        public (ControlFlowState, int) EnterFunction()
        {
            var previousState = (FlowState, LoopDepth);
            FlowState = ControlFlowState.Normal;
            LoopDepth = 0;
            return previousState;
        }

        public void Restore((ControlFlowState flowState, int loopDepth) context)
        {
            (FlowState, LoopDepth) = context;
        }

        public Value GetReturnValue()
        {
            var returnValue = (FlowState == ControlFlowState.Return) ? LastReturnValue : Value.None();
            FlowState = ControlFlowState.Normal; // Reset flow state after getting value
            return returnValue;
        }
    }
}