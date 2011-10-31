﻿namespace NMock2 {
    public interface IStates : ISelfDescribing {
        State Is(string state);
        void StartAs(string s);
        IStatePredicate IsNot(string state);
    }
}