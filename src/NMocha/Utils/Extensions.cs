using System;

namespace NMocha.Utils {
    public static class Extensions {
        public static TimeSpan Seconds(this int seconds) {
            return new TimeSpan(0,0,0,1);
        }
    }
}