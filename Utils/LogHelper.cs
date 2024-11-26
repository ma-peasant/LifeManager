using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LifeManager.Utils
{
    public class LogHelper
    {
        public const int enabledLogLvl = LOG_LEVEL_DEFAULT;
        public const int LOG_LEVEL_DEFAULT = 2;
        public const int LOG_LEVEL_ERROR = 4;
        public const int LOG_LEVEL_HIGH = 3;
        public const int LOG_LEVEL_LOCAL_DB = 5;
        public const int LOG_LEVEL_LOW = 1;
        private const int APP_LOG_MAX_SIZE = 100 * 1024;
        private const int DEBUG_LOG_MAX_SIZE = 30 * 1024 * 1024;
        private const int ERROR_LOG_MAX_SIZE = 15 * 1024 * 1024;
        private static string _applogPath = string.Empty;
        private static object _lock = new object();
        private static string _logPath = string.Empty;

        //上传日志大小触发条件
        private System.Timers.Timer _checkHistoryTimer;

        public LogHelper(string logPath, string applogPath)
        {
            _logPath = logPath;
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            // 增加applog文件存放路径
            _applogPath = applogPath;
            if (!Directory.Exists(_applogPath))
            {
                Directory.CreateDirectory(_applogPath);
            }

            _checkHistoryTimer = new System.Timers.Timer();
            _checkHistoryTimer.Interval = 1000;
            _checkHistoryTimer.Elapsed += timer_Elapsed;
            _checkHistoryTimer.Start();
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum AppLogLevelEnum
            {
                /// <summary>
                /// system is unusable  # 紧急情况 系统无法使用
                /// </summary>
                emergency = 0,
                /// <summary>
                /// action must be taken immediately  # 警报 必须立即采取措施修复
                /// </summary>
                alert = 1,
                /// <summary>
                /// critical conditions  # 紧急/严重 情况
                /// </summary>
                critical = 2,
                /// <summary>
                /// error conditions  # 错误
                /// </summary>
                error = 3,
                /// <summary>
                /// warning conditions  # 警告
                /// </summary>
                warning = 4,
                /// <summary>
                /// normal but significant condition  # 注意
                /// </summary>
                notice = 5,
                /// <summary>
                /// informational messages  # 正常信息
                /// </summary>
                info = 6,
                /// <summary>
                /// debug-level messages  # 调试
                /// </summary>
                debug = 7
            }
        /// <summary>
        /// 检查历史未上传日志
        /// </summary>
        public static void CheckHistoryLog()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(_applogPath);
                string todayLogName = "AppLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

                string uploadDir = Path.Combine(_applogPath, "Upload");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);
                var logs = dir.GetFiles();
                //异步上传日志
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        foreach (var hisLog in logs)
                        {
                            string destinationPath = uploadDir + "\\" + hisLog.Name.Substring(0, hisLog.Name.Length - 4) + "_" + DateTime.Now.ToString("HH-mm-ss-fff") + ".txt";

                            File.Move(hisLog.FullName, destinationPath);
                            Thread.Sleep(300);
                        }
                    }
                    catch
                    {
                    }

                }, TaskCreationOptions.PreferFairness);
            }
            catch (Exception ex)
            {
                //WriteDebugLog(LogHelper.LOG_LEVEL_ERROR, "LogHelper CheckHistoryLog异常： " + ex.Message);
            }

        }

        /// <summary>
        /// 上传并处理日志文件
        /// </summary>
        /// <param name="uploadDir"></param>
        /// <param name="destinationPath"></param>
        private static void UploadAndDealLog(string uploadDir, string destinationPath)
        {
            try
            {
                //检查是否由未上传或上传失败的文件
                DirectoryInfo uploadPath = new DirectoryInfo(uploadDir);
                var files = uploadPath.GetFiles().Where(p => p.Name.StartsWith("AppLog_"));
                var uploadfiles = files.Count() > 10 ? files.Take(10) : files;
                //foreach (var needUploadFile in uploadfiles)
                //{
                //    string response = FileUploadHelper.HttpUploadFile(Constants.UploadLogURL, needUploadFile.FullName, null, null);
                //    var ojson = JsonHelper.JsonDeserialize<JsonModel.ObjCode>(response);
                //    if (ojson != null && ojson.code == "0")
                //    {
                //        string filename = Path.GetFileName(needUploadFile.FullName);
                //        needUploadFile.Delete();//上传成功后删除文件
                //        Thread.Sleep(300);
                //    }
                //}
            }
            catch
            {
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
                // 得到 hour minute second  如果等于某个值就开始执行
                int intHour = e.SignalTime.Hour;
                int intMinute = e.SignalTime.Minute;
                int intSecond = e.SignalTime.Second;
                // 定制时间,在17：00：00 的时候执行
                int iHour = 17;
                int iMinute = 00;
                int iSecond = 00;
                // 设置 每天的17：00：00检查
                if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
                {
                    CheckHistoryLog();
                }
            }
            #region Text Log
            /// <summary>
            /// 写入日志
            /// </summary>
            /// <param name="level"></param>
            /// <param name="message"></param>
            public static void WriteDebugLog(int level, string message)
            {
                string msg = "";

                if (level < enabledLogLvl)
                {
                    return;
                }
                string applevel = "";

                switch (level)
                {
                    case LogHelper.LOG_LEVEL_LOW:
                        msg = "【LOW    】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.notice.ToString();
                        break;
                    case LogHelper.LOG_LEVEL_HIGH:
                        msg = "【HIGH   】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.warning.ToString();
                        break;
                    case LogHelper.LOG_LEVEL_ERROR:
                        msg = "【ERROR  】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.error.ToString();
                        break;
                    default:
                        msg = "【DEFAULT】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.info.ToString();
                        break;

                }
                lock (_lock)
                {
                    WriteDebugLog(msg, applevel, null, null, null);
                }
                //Console.WriteLine(msg);

            }

            /// <summary>
            /// http请求日志
            /// </summary>
            /// <param name="level"></param>
            /// <param name="message"></param>
            /// <param name="method"></param>
            /// <param name="url"></param>
            public static void WriteDebugLog(int level, string message, string method, string url, string param)
            {
                string msg = "";

                if (level < enabledLogLvl)
                {
                    return;
                }
                string applevel = "";

                switch (level)
                {
                    case LogHelper.LOG_LEVEL_LOW:
                        msg = "【LOW    】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.notice.ToString();
                        break;
                    case LogHelper.LOG_LEVEL_HIGH:
                        msg = "【HIGH   】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.warning.ToString();
                        break;
                    case LogHelper.LOG_LEVEL_ERROR:
                        msg = "【ERROR  】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.error.ToString();
                        break;
                    default:
                        msg = "【DEFAULT】 " + message;
                        applevel = LogHelper.AppLogLevelEnum.info.ToString();
                        break;

                }
                lock (_lock)
                {
                    WriteDebugLog(msg, applevel, method, url, param);
                }

                //Console.WriteLine(msg);

            }
        public static void WriteTextErrorLog(string message)
        {
            StreamWriter eventFile = null;
            DateTime now = DateTime.Now;
            if (_logPath.Length == 0)
            {
                _logPath = Constants.LogFolderPath;
            }
            string logFilePath;
            try
            {
                logFilePath = _logPath + "CrashLog_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
                if (File.Exists(logFilePath))
                {
                    FileInfo fileInfo = new FileInfo(logFilePath);
                    if (fileInfo.Length >= ERROR_LOG_MAX_SIZE)
                    {
                        if (!Directory.Exists(Path.Combine(_logPath, "History")))
                            Directory.CreateDirectory(Path.Combine(_logPath, "History"));

                        string destinationPath = _logPath + "History" + "\\" + fileInfo.Name.Substring(0, fileInfo.Name.Length - 4) + "_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
                        File.Move(logFilePath, destinationPath);
                    }
                }

                eventFile = new StreamWriter(logFilePath, true);
                eventFile.Write(now.ToString("MM/dd/yyyy HH:mm:ss") + " \t" + message + "\r\n");
                eventFile.Flush();
            }
            catch (Exception ex)
            {
                // WriteDebugLog(LogHelper.LOG_LEVEL_ERROR, "LogHelper WriteTextErrorLog异常： " + ex.Message);
            }
            finally
            {
                if (eventFile != null) { eventFile.Close(); eventFile = null; }
                //this.mutex.ReleaseMutex();
            }
        }

        private static void WriteDebugLog(string message, string applevel, string method, string url, string param)
        {
                DateTime now = DateTime.Now;
                if (_logPath.Length == 0)
                {
                    _logPath = Constants.LogFolderPath;
                }
                // 原有log文件写入
                string logFilePath;
                StreamWriter eventFile = null;
                try
                {
                    //logFilePath = _logPath + "DebugLog"+now.DayOfWeek.ToString() + ".txt";
                    logFilePath = _logPath + "TraceLog_" + now.ToString("yyyy-MM-dd") + ".txt";
                    if (File.Exists(logFilePath))
                    {
                        FileInfo fileInfo = new FileInfo(logFilePath);
                        if (fileInfo.Length >= DEBUG_LOG_MAX_SIZE)
                        {
                            string historyDir = Path.Combine(_logPath, "History");
                            if (!Directory.Exists(historyDir))
                                Directory.CreateDirectory(historyDir);

                            string destinationPath = _logPath + "History" + "\\" + fileInfo.Name.Substring(0, fileInfo.Name.Length - 4) + "_" + now.ToString("yyyy-MM-dd") + ".txt";

                            File.Move(logFilePath, destinationPath);

                            foreach (string overdueFilePath in Directory.GetFiles(historyDir))
                            {
                                FileInfo overdueFile = new FileInfo(overdueFilePath);
                                if (overdueFile.CreationTime < DateTime.Now.AddDays(-15))
                                {
                                    overdueFile.Delete();
                                }
                            }
                        }
                    }
                    eventFile = new StreamWriter(logFilePath, true);
                    eventFile.Write(now.ToString("MM/dd/yyyy HH:mm:ss") + " \t" + message + "\r\n");
                    eventFile.Flush();
                }
                catch (Exception ex)
                {
                    //WriteDebugLog(LogHelper.LOG_LEVEL_ERROR, "LogHelper WriteDebugLog异常： " + ex.Message);
                }
                finally
                {
                    if (eventFile != null) { eventFile.Close(); eventFile = null; }
                    //this.mutex.ReleaseMutex();
                }


                // applog文件写入
                //if (_applogPath.Length == 0)
                //{
                //    _applogPath = Constants.AppLogFolderPath;
                //}
                //AppLogModel applogInfo = new AppLogModel();
                //LogContentModel logContent = new LogContentModel();
                //StreamWriter appeventFile = null;
                //logContent.level = applevel;
                //logContent.details = new LogDetails()
                //{
                //    log = message
                //};
                //// 基础信息
                //logContent.app = Constants.AppName;
                //logContent.dept_id = Common.DEPT_ID;
                //logContent.doc_id = Common.DoctorID;
                //logContent.hosp_id = Common.HOSP_ID;
                //logContent.os = Common.OSVersion;
                //logContent.starttime = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                //logContent.version = Constants.VersionNum();
                //logContent.macaddress = Common.GetMacAddress();
                //logContent.method = method;
                //logContent.uri = url;
                //logContent.parameters = param;

                //applogInfo.winapp = logContent;
                //applogInfo.timestamp = now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                //// applog文件写入
                //string applogFilePath;
                //string filename = "AppLog_" + now.ToString("yyyy-MM-dd") + ".txt";
                //applogFilePath = _applogPath + filename;
                //try
                //{
                //    appeventFile = new StreamWriter(applogFilePath, true);
                //    appeventFile.Write(JsonConvert.SerializeObject(applogInfo).Replace("\"timestamp\"", "\"@timestamp\"") + "\n");
                //    appeventFile.Flush();

                //}
                //catch (Exception ex)
                //{
                //    //WriteDebugLog(LogHelper.LOG_LEVEL_ERROR, "LogHelper WriteDebugLog applog异常： " + ex.Message);
                //}
                //finally
                //{
                //    if (appeventFile != null) { appeventFile.Close(); appeventFile = null; }
                //    if (File.Exists(applogFilePath))
                //    {
                //        FileInfo fileInfo = new FileInfo(applogFilePath);
                //        if (fileInfo.Length >= APP_LOG_MAX_SIZE)
                //        {
                //            CheckHistoryLog();
                //        }
                //    }
                //}
            }
            #endregion
        }
}
