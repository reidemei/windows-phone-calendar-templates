using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;

namespace net.reidemeister.wp.CalendarTemplates.Data
{
    public class Model
    {
        public Model()
        {
            this.Templates = new SortedObservableCollection<CalendarTemplate>();
        }

        /// <summary>
        /// Alle Templates.
        /// </summary>
        public ObservableCollection<CalendarTemplate> Templates { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Lädt alle Daten des Models.
        /// </summary>
        public async Task LoadData()
        {
            if (IsDataLoaded) return;

            // load all books via the folders
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dataFolder = await local.CreateFolderAsync("templates", CreationCollisionOption.OpenIfExists);
            foreach (var file in await dataFolder.GetFilesAsync())
            {
                CalendarTemplate template = await CalendarTemplate.Load(dataFolder, file);
                if (template != null)
                {
                    this.Templates.Add(template);
                }
                else
                {
#if DEBUG
                    Debug.WriteLine("removing invalid file: " + file.Name);
#endif
                    // invalid folder - remove
                    await file.DeleteAsync();
                }
            }
            this.IsDataLoaded = true;
        }
    }
}
