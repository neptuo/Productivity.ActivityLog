using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    /// <summary>
    /// Arguments for event when process or title has changed.
    /// The event is determined by <see cref="Type"/>.
    /// 
    /// If the event type is <see cref="ProcessChangedType.Process"/>, both <see cref="OriginalProcessId"/> and <see cref="CurrentProcessId"/> are set.
    /// If the event type is <see cref="ProcessChangedType.Title"/>, only <see cref="CurrentProcessId"/> is set.
    /// </summary>
    public class ProcessChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a type of the event.
        /// </summary>
        public ProcessChangedType Type { get; private set; }

        /// <summary>
        /// Gets an id of previous foreground process.
        /// </summary>
        public int? OriginalProcessId { get; private set; }

        /// <summary>
        /// Gets an id of current foreground process.
        /// </summary>
        public int? CurrentProcessId { get; private set; }

        /// <summary>
        /// Gets a title of previous foreground window.
        /// </summary>
        public string OriginalTitle { get; private set; }

        /// <summary>
        /// Gets a title of current foreground window.
        /// </summary>
        public string CurrentTitle { get; private set; }

        public string OriginalPath { get; private set; }
        public string CurrentPath { get; private set; }

        /// <summary>
        /// Creates a new instance for the event type <see cref="ProcessChangedType.Process"/>.
        /// </summary>
        /// <param name="originalProcessId">An id of previous foreground process.</param>
        /// <param name="currentProcessId">An id of current foreground process.</param>
        /// <param name="originalPath">A path to an executable of previous foreground process.</param>
        /// <param name="currentPath">A path to and executable of current foreground process.</param>
        /// <param name="originalTitle">A title of previous foreground window.</param>
        /// <param name="currentTitle">A title of current foreground window.</param>
        public static ProcessChangedEventArgs ForProcessChange(int originalProcessId, int currentProcessId, string originalPath, string currentPath, string originalTitle, string currentTitle)
        {
            return new ProcessChangedEventArgs()
            {
                Type = ProcessChangedType.Process,
                OriginalProcessId = originalProcessId,
                CurrentProcessId = currentProcessId,
                OriginalPath = originalPath,
                CurrentPath = currentPath,
                OriginalTitle = originalTitle,
                CurrentTitle = currentTitle,
            };
        }

        /// <summary>
        /// Creates a new instance for the event type <see cref="ProcessChangedType.Process"/>.
        /// </summary>
        /// <param name="originalProcessId">An id of previous foreground process.</param>
        /// <param name="currentProcessId">An id of current foreground process.</param>
        /// <param name="originalPath">A path to an executable of previous foreground process.</param>
        /// <param name="currentPath">A path to and executable of current foreground process.</param>
        /// <param name="originalTitle">A title of previous foreground window.</param>
        /// <param name="currentTitle">A title of current foreground window.</param>
        public static ProcessChangedEventArgs ForProcessChange(int currentProcessId, string currentPath, string currentTitle)
        {
            return new ProcessChangedEventArgs()
            {
                Type = ProcessChangedType.Process,
                CurrentProcessId = currentProcessId,
                CurrentPath = currentPath,
                CurrentTitle = currentTitle,
            };
        }

        /// <summary>
        /// Create a new instance for the event type <see cref="ProcessChangedType.Title"/>.
        /// </summary>
        /// <param name="currentProcessId">An id of current foreground process.</param>
        /// <param name="originalTitle">A title of previous foreground window.</param>
        /// <param name="currentTitle">A title of current foreground window.</param>
        public static ProcessChangedEventArgs ForTitleChange(int currentProcessId, string originalTitle, string currentTitle)
        {
            return new ProcessChangedEventArgs()
            {
                Type = ProcessChangedType.Title,
                CurrentProcessId = currentProcessId,
                OriginalTitle = originalTitle,
                CurrentTitle = currentTitle,
            };
        }
    }
}
