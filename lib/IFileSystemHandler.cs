using System;
using System.IO;

namespace mooftpserv
{
    /// <summary>
    /// File system entry as returned by List.
    /// </summary>
    public struct FileSystemEntry
    {
        public string Name;
        public bool IsDirectory;
        public long Size;
        public DateTime LastModifiedTimeUtc;
    }

    /// <summary>
    /// Wrapper that either contains a value or an error string.
    /// </summary>
    public class ResultOrError<T>
    {
        private T result;
        private string error;

        private ResultOrError(T result, string error)
        {
            this.result = result;
            this.error = error;
        }

        public static ResultOrError<T> MakeResult(T result)
        {
            return new ResultOrError<T>(result, null);
        }

        public static ResultOrError<T> MakeError(string error)
        {
            if (error == null)
                throw new ArgumentNullException();
            return new ResultOrError<T>(default(T), error);
        }

        public bool HasError
        {
            get { return error != null; }
        }

        public string Error
        {
            get { return error; }
        }

        public T Result
        {
            get
            {
                if (HasError)
                    throw new InvalidOperationException(String.Format("No result available, error: {0}", error));
                return result;
            }
        }
    };

    /// <summary>
    /// Interface for file system access from FTP.
    /// </summary>
    public interface IFileSystemHandler
    {
        /// <summary>
        /// Clone this instance. Each FTP session uses a separate, cloned instance.
        /// </summary>
        IFileSystemHandler Clone();

        /// <summary>
        /// PWD: Returns the path of the current working directory.
        /// </summary>
        /// <returns>
        /// The absolute path of the current directory or an error string.
        /// </returns>
        ResultOrError<string> GetCurrentDirectory();

        /// <summary>
        /// CWD: Changes the current directory.
        /// </summary>
        /// <returns>
        /// The new absolute path or an error string.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path to which to change.
        /// </param>
        ResultOrError<string> ChangeDirectory(string path);

        /// <summary>
        /// CDUP: Changes the current directory to the parent directory.
        /// </summary>
        /// <returns>
        /// The new absolute path or an error string.
        /// </returns>
        ResultOrError<string> ChangeToParentDirectory();

        /// <summary>
        /// MKD: Create a directory.
        /// </summary>
        /// <returns>
        /// The absolute path of the created directory or an error string.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path for the new directory.
        /// </param>
        ResultOrError<string> CreateDirectory(string path);

        /// <summary>
        /// RMD: Remove a directory.
        /// </summary>
        /// <returns>
        /// A bool or an error string. The bool is not actually used.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path for the directory.
        /// </param>
        ResultOrError<bool> RemoveDirectory(string path);

        /// <summary>
        /// RETR: Open a stream for reading the specified file.
        /// </summary>
        /// <returns>
        /// An opened stream for reading from the file, or an error string.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path for the file.
        /// </param>
        ResultOrError<Stream> ReadFile(string path);

        /// <summary>
        /// STOR: Open a stream for writing to the specified file.
        /// If the file exists, it should be overwritten.
        /// </summary>
        /// <returns>
        /// An opened stream for writing to the file, or an error string.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path for the file.
        /// </param>
        ResultOrError<Stream> WriteFile(string path);

        /// <summary>
        /// DELE: Deletes a file.
        /// </summary>
        /// <returns>
        /// A bool or an error string. The bool is not actually used.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path for the file.
        /// </param>
        ResultOrError<bool> RemoveFile(string path);

        /// <summary>
        /// LIST: Return a list of files and folders in the current directory, or the optionally specified path.
        /// </summary>
        /// <param name='path'>
        /// An array of file system entries or an error string.
        /// </param>
        ResultOrError<FileSystemEntry[]> ListEntries(string path = null);

        /// <summary>
        /// SIZE: Gets the size of a file in bytes.
        /// </summary>
        /// <returns>
        /// The file size, or -1 on error.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path.
        /// </param>
        ResultOrError<long> GetFileSize(string path);

        /// <summary>
        /// MDTM: Gets the last modified timestamp of a file.
        /// </summary>
        /// <returns>
        /// The last modified time in UTC, or an error string.
        /// </returns>
        /// <param name='path'>
        /// A relative or absolute path.
        /// </param>
        ResultOrError<DateTime> GetLastModifiedTimeUtc(string path);
    }
}

