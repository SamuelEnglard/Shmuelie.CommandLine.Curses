using Option = System.CommandLine.Option;

namespace Shmuelie.CommandLine.Curses.Runner
{
    /// <summary>
    ///     Extensions to <see cref="Option"/>.
    /// </summary>
    public static class OptionExtensions
    {
        /// <summary>
        ///     Add an Alias to an <see cref="Option"/>.
        /// </summary>
        /// <param name="this">The <see cref="Option"/> to add an alias to.</param>
        /// <param name="alias">The alias to add.</param>
        /// <returns><paramref name="this"/>.</returns>
        public static Option BuildAlias(this Option @this, string alias)
        {
            @this.AddAlias(alias);
            return @this;
        }
    }
}
