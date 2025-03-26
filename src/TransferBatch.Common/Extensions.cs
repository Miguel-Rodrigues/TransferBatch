using System.Security.Cryptography;
using System.Text;

namespace TransferBatch.Common
{
    public static class Extensions
    {
        public static Dictionary<string, string> ToParameters(this string[] args, params string[] switchParameters)
        {
            var parameters = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i += 2)
            {
                var parameter = args[i].Remove(0, 1);

                // Check if the parameter is a switch
                if (switchParameters.Contains(parameter, StringComparer.InvariantCultureIgnoreCase))
                {
                    i--;
                    parameters.Add(parameter.ToUpper(), bool.TrueString);
                    continue;
                }
                else
                {
                    if (i == args.Length - 1)
                    {
                        // The last argument is a value without a parameter
                        parameters.Add(string.Empty, parameter);
                    }
                    else
                    {
                        parameters.Add(parameter.ToUpper(), args[i + 1]);
                    }
                }
            }

            return parameters;
        }
    }
}
