using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KEngine
{
    public class LogFileRecorder
    {
        private StreamWriter writer;
        /// <summary>
        /// 初始化记录器，在游戏退出时调用Close
        /// </summary>
        /// <param name="filePath">文件名中不能包含特殊字符比如:</param>
        /// <param name="mode"></param>
        public LogFileRecorder(string filePath, FileMode mode = FileMode.Create)
        {
            int index = 0;
            try
            {
                var fs = new FileStream(filePath, mode);
                writer = new StreamWriter(fs);
            }
            catch (IOException e)
            {
                filePath = Path.GetDirectoryName(filePath) + "/" + Path.GetFileNameWithoutExtension(filePath) + "_" + (index++) + Path.GetExtension(filePath);
				Debug.LogError(e.Message);
            }
        }

        public void WriteLine(string line)
        {
            writer.WriteLine(line);
            writer.Flush();
        }

        public void Close()
        {
            writer.Flush();
            writer.Close();
        }


        #region 记录函数执行耗时

        private static Dictionary<string, LogFileRecorder> loggers = new Dictionary<string, LogFileRecorder>();

        public static void CloseStream()
        {
            foreach (var kv in loggers)
            {
                kv.Value.Close();
            }
        }
        
        public static void WriteProfileLog(string logType, string line)
        {
            LogFileRecorder logger;
            if (!loggers.TryGetValue(logType, out logger))
            {
                logger = new LogFileRecorder(Application.persistentDataPath + "/profiler_" + logType + ".csv");
                loggers.Add(logType, logger);

                if (logType == "UI")
                {
                    logger.WriteLine("UI Name,Operation,Cost(ms)");
                }
                else
                {
                    logger.WriteLine("");
                }
            }

            logger.WriteLine(line);
        }

        #endregion
    }
}