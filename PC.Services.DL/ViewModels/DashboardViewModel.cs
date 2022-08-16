using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.ViewModels
{
    public class DashboardViewModel
    {
        /// <summary>
        /// Gets or sets the count account.
        /// </summary>
        /// <value>The count account.</value>
        public int CountAccount { get; set; }
        /// <summary>
        /// Gets or sets the count roles.
        /// </summary>
        /// <value>The count roles.</value>
        public int CountRoles { get; set; }
        /// <summary>
        /// Gets or sets the count permissions.
        /// </summary>
        /// <value>The count permissions.</value>
        public int? CountPermissions { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        //public Settings Settings { get; set; }


    }
}
