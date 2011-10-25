//-----------------------------------------------------------------------
// <copyright file="Example.cs" company="NMock2">
//
//   http://www.sourceforge.net/projects/NMock2
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// This is the easiest way to ignore StyleCop rules on this file, even if we shouldn't use this tag:
// <auto-generated />
//-----------------------------------------------------------------------
namespace NMock2.AcceptanceTests
{
    using NUnit.Framework;

    public delegate void WhoIsThereResponse();
    public delegate void WhoResponse(string firstName);

    public interface IKnockKnock
    {
        void KnockKnock(IJoker joker);
        void TellFirstName(IJoker joker, string firstName);
        void TellPunchline(IJoker joker, string punchline);
    }
    
    public interface IJoker
    {
        void Respond(string response);
        
        void Ha();
        void Hee();
        void Ho();
    }
    
    public class Audience : IKnockKnock
    {
        public void KnockKnock(IJoker joker)
        {
            joker.Respond("Who's there?");
        }
        
        public void TellFirstName(IJoker joker, string firstName)
        {
            joker.Respond(firstName + ", who?");
        }

        public void TellPunchline(IJoker joker, string punchLine)
        {
            joker.Ha();
            joker.Ha();
            joker.Hee();
            joker.Ho();
            joker.Ho();
            joker.Hee();
            joker.Hee();
        }
    }
    
    [TestFixture]
    public class Example : AcceptanceTestBase
    {
        [Test]
        public void KnockKnockJoke()
        {
            const string firstName = "Doctor";
            const string punchline = "How did you know?";
			IJoker joker = (IJoker)Mocks.NewNamedMock(typeof(IJoker), "joker");
            Audience audience = new Audience();

            using (Mocks.Ordered)
            {
                Expect.Once.On(joker).Message("Respond").With(Is.EqualTo("Who's there?"));
                Expect.Once.On(joker).Message("Respond").With(Is.StringContaining(firstName) & Is.StringContaining("who?"));

                using (Mocks.Unordered)
                {
                    Expect.AtLeastOnce.On(joker).Message("Ha");
                    Expect.AtLeastOnce.On(joker).Message("Ho");
                    Expect.AtLeastOnce.On(joker).Message("Hee");
                }
            }
            
            audience.KnockKnock(joker);
            audience.TellFirstName(joker, firstName);
            audience.TellPunchline(joker, punchline);
        }

        [Test]
        public void KnockKnockJoke_UsingDefaultExpectation()
        {
            const string firstName = "Doctor";
            const string punchline = "How did you know?";
			IJoker joker = (IJoker)Mocks.NewNamedMock(typeof(IJoker), "joker");
            Audience audience = new Audience();

            using (Mocks.Ordered)
            {
                Expect.Once.On(joker).Message("Respond").With(Is.EqualTo("Who's there?"));
                Expect.Once.On(joker).Message("Respond").With(Is.StringContaining(firstName) & Is.StringContaining("who?"));

                using (Mocks.Unordered)
                {
                    // I use directly On instead of AtLeastOnce.On
                    Expect.On(joker).Message("Ha");
                    Expect.On(joker).Message("Ho");
                    Expect.On(joker).Message("Hee");
                }
            }

            audience.KnockKnock(joker);
            audience.TellFirstName(joker, firstName);
            audience.TellPunchline(joker, punchline);
        }
    }
}
