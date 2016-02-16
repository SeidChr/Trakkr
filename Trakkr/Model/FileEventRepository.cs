using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Model
{
    [Export("TextFile", typeof(IEventRepository))]
    public class FileEventRepository : IEventRepository
    {
        private string GetTextLineFromEvent(IEvent @event)
        {
            throw new NotImplementedException();
        }

        private static IEvent GetEventFromTextLine(string line)
        {
            throw new NotImplementedException();
        }

        public void Store(IEnumerable<IEvent> events)
        {
            var eventListStartTime = events.FirstOrDefault()?.UtcTimestamp ?? DateTime.Now;
            var file = GetFile(eventListStartTime);
            var writer = file.CreateText();
            foreach (var @event in events)
            {
                writer.WriteLine(GetTextLineFromEvent(@event));
            }
        }

        public IEnumerable<IEvent> Load()
        {
            var file = GetFile(DateTime.Now);

            if (!file.Exists) yield break;

            var fileReader = file.OpenText();
            while (!fileReader.EndOfStream)
            {
                var line = fileReader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    yield return GetEventFromTextLine(line);
                }
            }
        }

        private static FileInfo GetFile(DateTime dateTime)
            => new FileInfo(Path.Combine(GetDirectoryPath(), GetFileName(dateTime)));

        private static string GetDirectoryPath()
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static string GetFileName(DateTime dateTime) 
            => $"Trakkr_{dateTime.ToLocalTime().ToString("d", DateTimeFormatInfo.InvariantInfo)}.txt";
    }
}
