namespace PhoneSeat.Logic;

public static class WindowsCommandLine
{
    public static string QuoteArgument(string argument)
    {
        ArgumentNullException.ThrowIfNull(argument);

        if (argument.Length == 0)
            return "\"\"";

        if (!argument.Any(c => char.IsWhiteSpace(c) || c == '"'))
            return argument;

        var sb = new StringBuilder();
        sb.Append('"');
        var backslashes = 0;

        foreach (var c in argument)
        {
            if (c == '\\')
            {
                backslashes++;
                continue;
            }

            if (c == '"')
            {
                sb.Append('\\', backslashes * 2 + 1);
                sb.Append('"');
                backslashes = 0;
                continue;
            }

            if (backslashes > 0)
            {
                sb.Append('\\', backslashes);
                backslashes = 0;
            }

            sb.Append(c);
        }

        if (backslashes > 0)
            sb.Append('\\', backslashes * 2);

        sb.Append('"');
        return sb.ToString();
    }

    public static string BuildExecutableCommand(string executable, string? rawArguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executable);
        var command = QuoteArgument(executable.Trim());
        return string.IsNullOrWhiteSpace(rawArguments)
            ? command
            : command + " " + rawArguments.Trim();
    }
}
