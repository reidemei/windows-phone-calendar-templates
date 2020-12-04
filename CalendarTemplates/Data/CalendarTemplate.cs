using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using net.reidemeister.wp.CalendarTemplates.Resources;
using System.Xml;
using Windows.Storage;
using System.Text;
using System.Net;
using Microsoft.Phone.Shell;
using System.Collections.Generic;

namespace net.reidemeister.wp.CalendarTemplates.Data
{
    public class CalendarTemplate : INotifyPropertyChanged
    {
        private const int TIME_CURRENT = 0;
        private const int TIME_FIXED = 1;
        private const int TIME_CURRENT_OFFSET = 2;
        private const int TIME_ALL_DAY = 3;

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

#if DOES_NOT_WORK // keine Anzeige in der GUI
        #region DataError
        private Dictionary<String, List<String>> errors = new Dictionary<string, List<string>>();

        // Adds the specified error to the errors collection if it is not 
        // already present, inserting it in the first position if isWarning is 
        // false. Raises the ErrorsChanged event if the collection changes. 
        public void AddError([CallerMemberName] string propertyName = "", string error = "invalid value", bool isWarning = false)
        {
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = new List<string>();

            if (!errors[propertyName].Contains(error))
            {
                if (isWarning) errors[propertyName].Add(error);
                else errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        // Removes the specified error from the errors collection if it is
        // present. Raises the ErrorsChanged event if the collection changes.
        public void RemoveError([CallerMemberName] string propertyName = "", string error = "invalid value")
        {
            if (errors.ContainsKey(propertyName) && errors[propertyName].Contains(error))
            {
                errors[propertyName].Remove(error);
                if (errors[propertyName].Count == 0) errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null) ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName)) return null;
            return errors[propertyName];
        }

        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        #endregion
#endif
        public string ID { get; set; }

        private string icon = "clock.png";
        public string Icon
        {
            get
            {
                return (this.icon);
            }
            set
            {
                if (value != null)
                {
                    this.icon = value;
                }
                NotifyPropertyChanged();
            }
        }

        private string name = "";
        public string Name
        {
            get
            {
                return (this.name);
            }
            set
            {
                this.name = value ?? "";
                NotifyPropertyChanged();
            }
        }

        private string subject = "";
        public string Subject
        {
            get
            {
                return (this.subject);
            }
            set
            {
                this.subject = value ?? "";
                NotifyPropertyChanged();
            }
        }
        
        private string location = "";
        public string Location 
        {
            get
            {
                return (this.location);
            }
            set
            {
                this.location = value ?? "";
                NotifyPropertyChanged();
            }
        }

        private int time = TIME_CURRENT;
        public int Time 
        {
            get
            {
                return (this.time);
            }
            set
            {
                if (value >= TIME_CURRENT && value <= TIME_ALL_DAY)
                {
                    this.time = value;
                }
                NotifyPropertyChanged();
            }
        }

        private DateTime fixedStartTime = new DateTime(1, 1, 1, 8, 0, 0);
        public DateTime FixedStartTime
        {
            get
            {
                return (this.fixedStartTime);
            }
            set
            {
                if (value != null)
                {
                    this.fixedStartTime = value;
                }
                NotifyPropertyChanged();
            }
        }

        private int timeStartOffsetMinutes = 0;
        public int TimeStartOffsetMinutes
        {
            get
            {
                return (this.timeStartOffsetMinutes);
            }
            set
            {
                if (value >= 0)
                {
                    this.timeStartOffsetMinutes = value;
                }
                NotifyPropertyChanged();
            }
        }

        private int timeEndOffsetDays = 0;
        public int TimeEndOffsetDays
        {
            get
            {
                return (this.timeEndOffsetDays);
            }
            set
            {
                if (value >= 0)
                {
                    this.timeEndOffsetDays = value;
                }
                NotifyPropertyChanged();
            }
        }

        private int timeEndOffsetHours = 1;
        public int TimeEndOffsetHours
        {
            get
            {
                return (this.timeEndOffsetHours);
            }
            set
            {
                if (value >= 0)
                {
                    this.timeEndOffsetHours = value;
                }
                NotifyPropertyChanged();
            }
        }

        private int timeEndOffsetMinutes = 0;
        public int TimeEndOffsetMinutes
        {
            get
            {
                return (this.timeEndOffsetMinutes);
            }
            set
            {
                if (value >= 0)
                {
                    this.timeEndOffsetMinutes = value;
                }
                NotifyPropertyChanged();
            }
        }

        private int status = 2;
        public int Status
        {
            get
            {
                return (this.status);
            }
            set
            {
                if (value >= 0 && value <= 3)
                {
                    this.status = value;
                }
                NotifyPropertyChanged();
            }
        }

        private int reminder = 0;
        public int Reminder
        {
            get
            {
                return (this.reminder);
            }
            set
            {
                if (value >= 0 && value <= 9)
                {
                    this.reminder = value;
                }
                NotifyPropertyChanged();
            }
        }

        private string note = "";
        public string Note
        {
            get
            {
                return (this.note);
            }
            set
            {
                this.note = value ?? "";
                NotifyPropertyChanged();
            }
        }

        internal async Task Save()
        {
#if DEBUG
            DateTime now = DateTime.Now;
#endif
            XElement root = new XElement("template");
            root.SetAttributeValue("id", this.ID);
            root.SetAttributeValue("icon", this.Icon);
            root.SetAttributeValue("name", this.Name);
            root.SetAttributeValue("subject", this.Subject);
            root.SetAttributeValue("location", this.Location);
            root.SetAttributeValue("time", this.Time.ToString());
            root.SetAttributeValue("fixedStartTime", this.FixedStartTime.ToString(CultureInfo.InvariantCulture));
            root.SetAttributeValue("timeStartOffsetMinutes", this.TimeStartOffsetMinutes);
            root.SetAttributeValue("timeEndOffsetDays", this.TimeEndOffsetDays);
            root.SetAttributeValue("timeEndOffsetHours", this.TimeEndOffsetHours);
            root.SetAttributeValue("timeEndOffsetMinutes", this.TimeEndOffsetMinutes);
            root.SetAttributeValue("reminder", this.Reminder);
            root.SetAttributeValue("status", this.Status);
            root.SetAttributeValue("note", this.Note);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dataFolder = await local.CreateFolderAsync("templates", CreationCollisionOption.OpenIfExists);
            using (var s = await dataFolder.OpenStreamForWriteAsync(this.ID + ".xml", CreationCollisionOption.OpenIfExists))
            {
                using (XmlWriter xw = XmlWriter.Create(s, xws))
                {
                    root.WriteTo(xw);
                }
            }
#if DEBUG
            Debug.WriteLine("saving template '" + this.Name + "' to file '" + this.ID + "' took " + ((DateTime.Now.Ticks - now.Ticks) / 10000) + " ms");
            if (Debugger.IsAttached)
            {
                StringBuilder sb = new StringBuilder();
                using (XmlWriter xw = XmlWriter.Create(sb, xws))
                {
                    root.WriteTo(xw);
                }
                Debug.WriteLine("content: " + sb.ToString());
            }
#endif
        }

        internal async static Task<CalendarTemplate> Load(Windows.Storage.StorageFolder folder, Windows.Storage.StorageFile file)
        {
            try 
            {
#if DEBUG
                DateTime now = DateTime.Now;
#endif
                using (var s = await folder.OpenStreamForReadAsync(file.Name))
                {
                    XDocument content = XDocument.Load(s);
                    CalendarTemplate template = new CalendarTemplate();
                    template.ID = content.Root.Attribute("id").Value;
                    template.Icon = content.Root.Attribute("icon").Value;
                    template.Name = content.Root.Attribute("name").Value;
                    template.Subject = content.Root.Attribute("subject").Value;
                    template.Location = content.Root.Attribute("location").Value;
                    template.Time = int.Parse(content.Root.Attribute("time").Value);
                    template.FixedStartTime = DateTime.Parse(content.Root.Attribute("fixedStartTime").Value, CultureInfo.InvariantCulture);
                    template.TimeStartOffsetMinutes = int.Parse(content.Root.Attribute("timeStartOffsetMinutes").Value);
                    template.TimeEndOffsetDays = int.Parse(content.Root.Attribute("timeEndOffsetDays").Value);
                    template.TimeEndOffsetHours = int.Parse(content.Root.Attribute("timeEndOffsetHours").Value);
                    template.TimeEndOffsetMinutes = int.Parse(content.Root.Attribute("timeEndOffsetMinutes").Value);
                    template.Reminder = int.Parse(content.Root.Attribute("reminder").Value);
                    template.Status = int.Parse(content.Root.Attribute("status").Value);
                    template.Note = content.Root.Attribute("note").Value;
#if DEBUG
                    Debug.WriteLine("loading template '" + template.Name + "' from file '" + file.Name + "' took " + 
                        ((DateTime.Now.Ticks - now.Ticks) / 10000) + " ms");
#endif
                    return (template);
                }
            } 
            catch (Exception) {}
            return (null);
        }

        /// <summary>
        /// Kopiert alle Werte außer der ID.
        /// </summary>
        /// <param name="template">von wo die Werte kopiert werden</param>
        public void CopyFrom(CalendarTemplate template)
        {
            this.Icon = template.Icon;
            this.Name = template.Name;
            this.Subject = template.Subject;
            this.Location = template.Location;
            this.Time = template.Time;
            this.FixedStartTime = template.FixedStartTime;
            this.TimeStartOffsetMinutes = template.TimeStartOffsetMinutes;
            this.TimeEndOffsetDays = template.TimeEndOffsetDays;
            this.TimeEndOffsetHours = template.TimeEndOffsetHours;
            this.TimeEndOffsetMinutes = template.TimeEndOffsetMinutes;
            this.Reminder = template.Reminder;
            this.Status = template.Status;
            this.Note = template.Note;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return (true);
            if (!(obj is CalendarTemplate)) return (false);

            CalendarTemplate template = obj as CalendarTemplate;
            if (!this.Icon.Equals(template.Icon)) return (false);
            if (!this.Name.Equals(template.Name)) return (false);
            if (!this.Subject.Equals(template.Subject)) return (false);
            if (!this.Location.Equals(template.Location)) return (false);
            if (!this.FixedStartTime.Hour.Equals(template.FixedStartTime.Hour)) return (false);
            if (!this.FixedStartTime.Minute.Equals(template.FixedStartTime.Minute)) return (false);
            if (!this.TimeStartOffsetMinutes.Equals(template.TimeStartOffsetMinutes)) return (false);
            if (!this.TimeEndOffsetDays.Equals(template.TimeEndOffsetDays)) return (false);
            if (!this.TimeEndOffsetHours.Equals(template.TimeEndOffsetHours)) return (false);
            if (!this.TimeEndOffsetMinutes.Equals(template.TimeEndOffsetMinutes)) return (false);
            if (!this.Reminder.Equals(template.Reminder)) return (false);
            if (!this.Status.Equals(template.Status)) return (false);
            if (!this.Note.Equals(template.Note)) return (false);
            return (true);
        }

        public void Run()
        {
            Microsoft.Phone.Tasks.SaveAppointmentTask t = new Microsoft.Phone.Tasks.SaveAppointmentTask();
            t.Subject = this.Subject;
            t.Location = this.Location;
            DateTime start = DateTime.Now;
            if (this.Time == TIME_CURRENT_OFFSET)
            {
                start = start.AddMinutes(this.TimeStartOffsetMinutes);
            }
            else if (this.Time == TIME_FIXED)
            {
                start = new DateTime(start.Year, start.Month, start.Day, this.FixedStartTime.Hour, this.FixedStartTime.Minute, 0);
            }
            else if (this.Time == TIME_ALL_DAY)
            {
                t.IsAllDayEvent = true;
            }
            t.StartTime = start;
            DateTime end = start.AddDays(this.TimeEndOffsetDays);
            if (this.Time != TIME_ALL_DAY)
            {
                end = end.AddHours(this.TimeEndOffsetHours);
                end = end.AddMinutes(this.TimeEndOffsetMinutes);
            }
            t.EndTime = end;
            
            Microsoft.Phone.UserData.AppointmentStatus s = Microsoft.Phone.UserData.AppointmentStatus.Busy;
            switch (this.Status)
            {
                case (0): s = Microsoft.Phone.UserData.AppointmentStatus.Free; break;
                case (1): s = Microsoft.Phone.UserData.AppointmentStatus.Tentative; break;
                case (3): s = Microsoft.Phone.UserData.AppointmentStatus.OutOfOffice; break;
            }
            t.AppointmentStatus = s;
            Microsoft.Phone.Tasks.Reminder r = Microsoft.Phone.Tasks.Reminder.None;
            switch (this.Reminder)
            {
                case (1): r = Microsoft.Phone.Tasks.Reminder.AtStartTime; break;
                case (2): r = Microsoft.Phone.Tasks.Reminder.FiveMinutes; break;
                case (3): r = Microsoft.Phone.Tasks.Reminder.TenMinutes; break;
                case (4): r = Microsoft.Phone.Tasks.Reminder.FifteenMinutes; break;
                case (5): r = Microsoft.Phone.Tasks.Reminder.ThirtyMinutes; break;
                case (6): r = Microsoft.Phone.Tasks.Reminder.OneHour; break;
                case (7): r = Microsoft.Phone.Tasks.Reminder.EighteenHours; break;
                case (8): r = Microsoft.Phone.Tasks.Reminder.OneDay; break;
                case (9): r = Microsoft.Phone.Tasks.Reminder.OneWeek; break;
            }
            t.Reminder = r;
            t.Details = this.Note;
            t.Show();
        }

        #region Tile
        private string GetShellTileUri
        {
            get
            {
                return ("/RunTemplate.xaml?template=" + HttpUtility.UrlEncode(this.ID));
            }
        }

        private ShellTile GetShellTile
        {
            get
            {
                string uri = this.GetShellTileUri;
                List<ShellTile> pinnedtiles = ShellTile.ActiveTiles.ToList();
                return ((from t in pinnedtiles where uri.Equals(t.NavigationUri.ToString()) select t).FirstOrDefault());
            }
        }

        public bool HasTitle
        {
            get
            {
                return (this.GetShellTile != null);
            }
        }

        public void CreateTile()
        {
            ShellTile tile = this.GetShellTile;
            if (tile != null) 
            {
#if DEBUG
                if (Debugger.IsAttached) Debugger.Break();
#endif
                this.UpdateTile();
                return;
            }
            IconicTileData data = new IconicTileData()
            {
                IconImage = new Uri("/Assets/Icons/Tile/" + this.Icon, UriKind.Relative),
                SmallIconImage = new Uri("/Assets/Icons/Tile/" + this.Icon, UriKind.Relative),
                Title = this.Name
            };
            ShellTile.Create(new Uri(this.GetShellTileUri, UriKind.Relative), data, false);
        }

        public void UpdateTile()
        {
            ShellTile tile = this.GetShellTile;
            if (tile == null) return;
            IconicTileData data = new IconicTileData()
            {
                IconImage = new Uri("/Assets/Icons/Tile/" + this.Icon, UriKind.Relative),
                SmallIconImage = new Uri("/Assets/Icons/Tile/" + this.Icon, UriKind.Relative),
                Title = this.Name
            };
            tile.Update(data);
        }

        public void DeleteTile()
        {
            ShellTile tile = this.GetShellTile;
            if (tile != null)
            {
                tile.Delete();
            }
        }
        #endregion
    }
}
