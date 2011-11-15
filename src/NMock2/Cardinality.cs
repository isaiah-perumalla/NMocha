namespace NMocha {
    public  struct Cardinality : ISelfDescribing{
        private readonly int required;
        private readonly int maximum;
        public static readonly Cardinality AllowAny = AtLeast(0);
        private static readonly Cardinality NeverCardinality = new Cardinality(0,0);

        private Cardinality(int required, int maximum) {
            this.required = required;
            this.maximum = maximum;
        }

        public static Cardinality AtMost(int count) {
            return Between(0, count);
        }

        public static Cardinality Between(int required, int count) {
            return new Cardinality(required, count);
        }

        public static Cardinality AtLeast(int count) {
            return Between(count, int.MaxValue);
        }

        public static Cardinality Exactly(int count) {
            return Between(count, count);
        }

        public bool AllowsMoreInvocations(int callCount) {
            return callCount < maximum;
        }

        public bool IsSatisfied(int callCount) {
            return callCount >= required && callCount <= maximum;
        }

        public void DescribeOn(IDescription description) {
            if (Equals(AllowAny)) description.AppendText("allowed");
            else if (maximum == 1 && required == 1) DescribeExpected(description, "once");
            else if (maximum == int.MaxValue && required == 1) DescribeExpected(description, "atleast once", required);
            else if (maximum == int.MaxValue && required > 1) DescribeExpected(description, "atleast {0} times", required);
            else if (maximum == required && required > 1) DescribeExpected(description, "exactly {0} times", required);
            else if (0 == required && maximum > 0) DescribeExpected(description,"at most {0} times", maximum);
            else if (Equals( NeverCardinality)) DescribeExpected(description,"never");
           
            
        }

        private static void DescribeExpected(IDescription description, string atleastTimes, params object[] args) {
            description.AppendText("expected ")
                .AppendTextFormat(atleastTimes, args);
        }

        public static Cardinality Never() {
            return NeverCardinality;
        }
    }
}