/**
* Log.cs written by Mathias Bylund, 2016
*/

using UnityEngine;

namespace Util
{
    /// <summary>
    /// A wrapper of the Debug.Logger
    /// It has convinient methods for logging depending on the logLevel
    /// </summary>
    public static class Log
    {
        // The different logging levels
        // The first is the most verbose and the last the least
        enum LEVEL
        {  
            VERBOSE,
            INFO,
            WARNING,
            ERROR,
            NONE
        }

        [SerializeField]
        private static LEVEL m_logLevel = LEVEL.INFO;

        // Since we want the method to Debug.Log
        private static ILogger logger = Debug.logger;

        /// <summary>
        /// Formats a log string
        /// </summary>
        /// <param name="tag">Tag of the log message</param>
        /// <param name="color">Color of the tag</param>
        /// <param name="message">The log message</param>
        /// <returns></returns>
        private static string Format(string tag, string color, string message)
        {
            return string.Concat("<color='", color, "'>[", tag, "] </color>", "\nMessage: ", message, "\n");
        }

        /// <summary>
        ///  Info logs information that is good during development
        /// </summary>
        /// <param name="message">Message that should be logger</param>
        /// <param name="context">Object that the log concerns</param>
        public static void Info(string message, UnityEngine.Object context = null)
        {
            if (Log.m_logLevel <= LEVEL.INFO)
            {
                logger.Log(Format("INFO", "blue", message), context);
            }
        }

        /// <summary>
        ///  Logs warnings but not fatal errors
        /// </summary>
        /// <param name="message">Message that should be logger</param>
        /// <param name="context">Object that the log concerns</param>
        public static void Warning(string message, UnityEngine.Object context = null)
        {
            if(Log.m_logLevel <= LEVEL.WARNING)
            {
                logger.Log(Format("WARNING", "orange", message), context);
            }
        }

        /// <summary>
        ///  Logs errors that are fatal for the application
        /// </summary>
        /// <param name="message">Message that should be logger</param>
        /// <param name="context">Object that the log concerns</param>
        public static void Error(string message, UnityEngine.Object context = null)
        {
            if (Log.m_logLevel <= LEVEL.ERROR)
            {
                logger.Log(Format("ERROR", "red", message), context);
            }
        }

        /// <summary>
        ///  Logs verbose messages that are good when debugging
        /// </summary>
        /// <param name="message">Message that should be logger</param>
        /// <param name="context">Object that the log concerns</param>
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Verbose(string message, UnityEngine.Object context = null)
        {
            if (Log.m_logLevel <= LEVEL.VERBOSE)
            {
                logger.Log(Format("VERBOSE", "grey", message), context);
            }
        }
    
    }
}
