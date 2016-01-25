#region

using SimpleMvvmToolkit;

#endregion

namespace Umehluko.Tools.UI.Models
{
    /// <summary>
    /// The busy model.
    /// </summary>
    public class BusyModel : ModelBase<BusyModel>
    {
        /// <summary>
        /// The is busy.
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;
                this.NotifyPropertyChanged(s => s.isBusy);
            }
        }
    }
}