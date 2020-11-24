using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier
{
    public static class BaseLogicCalculator
    {
        public static WaypointData wData;
        public static Dictionary<string, string> macros;

        public static void Setup(WaypointData wData, Dictionary<string, string> macros)
        {
            BaseLogicCalculator.wData = wData;
            BaseLogicCalculator.macros = macros;
        }

        public static AdditiveLogicChain[] ParseToAdditiveChains(string name, string rawlogic)
        {
            string[] rpnLogic = ShuntingYard(rawlogic);
            string[][] chains = CombinationCalculator(rpnLogic);

            return chains.Select(c => new AdditiveLogicChain
            {
                target = name,
                waypoints = c.Where(wData.IsWaypoint).ToArray(),
                misc = c.Where(w => !wData.IsWaypoint(w)).ToArray()
            }).ToArray();
        }

        public static string[][] CombinationCalculator(string[] postfixLogic)
        {
            IEnumerable<IEnumerable<string>> combinations = (IEnumerable<IEnumerable<string>>)new List<List<string>>();
            Stack<IEnumerable<IEnumerable<string>>> pending = new Stack<IEnumerable<IEnumerable<string>>>();


            for (int i = 0; i < postfixLogic.Length; i++)
            {
                switch (postfixLogic[i])
                {
                    default:
                        pending.Push(new List<IEnumerable<string>> { new List<string> { postfixLogic[i] } });
                        break;

                    case "+":
                        pending.Push(pending.Pop().NondeterministicUnion(pending.Pop()));
                        break;

                    case "|":
                        pending.Push(pending.Pop().Union(pending.Pop()));
                        break;

                }
            }

            return pending.Count == 1 ? pending.Pop().Select(l => l.ToArray()).ToArray() 
                : pending.Count == 0 ? new string[][] { new string[]{ } } 
                : throw new ArgumentException("Invalid logic?");
        }



        public static string[] ShuntingYard(string infix)
        {
            int i = 0;
            Stack<string> stack = new Stack<string>();
            List<string> postfix = new List<string>();

            while (i < infix.Length)
            {
                string op = GetNextOperator(infix, ref i);

                // Easiest way to deal with whitespace between operators
                if (op.Trim(' ') == string.Empty)
                {
                    continue;
                }

                if (op == "+" || op == "|")
                {
                    while (stack.Count != 0 && (op == "|" || op == "+" && stack.Peek() != "|") && stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }

                    stack.Push(op);
                }
                else if (op == "(")
                {
                    stack.Push(op);
                }
                else if (op == ")")
                {
                    while (stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }

                    stack.Pop();
                }
                else
                {
                    // Parse macros
                    if (macros.TryGetValue(op, out string macro))
                    {
                        postfix.AddRange(ShuntingYard(macro));
                    }
                    else
                    {
                        postfix.Add(op);
                    }
                }
            }

            while (stack.Count != 0)
            {
                postfix.Add(stack.Pop());
            }

            return postfix.ToArray();
        }

        private static string GetNextOperator(string infix, ref int i)
        {
            int start = i;

            if (infix[i] == '(' || infix[i] == ')' || infix[i] == '+' || infix[i] == '|')
            {
                i++;
                return infix[i - 1].ToString();
            }

            while (i < infix.Length && infix[i] != '(' && infix[i] != ')' && infix[i] != '+' && infix[i] != '|')
            {
                i++;
            }

            return infix.Substring(start, i - start).Trim(' ');
        }

    }
}
