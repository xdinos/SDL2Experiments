namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// This is an implementation of GenericItem that has an additional id.
    /// </summary>
    public class StandardItem : GenericItem
    {
        /// <summary>
        /// 
        /// </summary>
        public StandardItem()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        public StandardItem(string text, int id = 0)
                : this(text, null, id)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="icon"></param>
        /// <param name="id"></param>
        public StandardItem(string text, string icon, int id = 0)
                :base(text,icon)
        {
            d_id = id;
        }

        /// <summary>
        /// Id of this item
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return d_id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetId(int val)
        {
            d_id = val;
        }

        protected int d_id;
    }
}