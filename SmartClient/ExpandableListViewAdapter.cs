using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SmartClient
{
    public class ExpandableListViewAdapter : BaseExpandableListAdapter
    {
        private Context context;
        private List<string> listGroup;
        private Dictionary<string, List<string>> lstChild;

        public ExpandableListViewAdapter(Context context, List<string> listGroup, Dictionary<string, List<string>> lstChild) 
        {
            this.context = context;
            this.listGroup = listGroup;
            this.lstChild = lstChild;
        }

        protected List<string> DataList { get; set; }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView==null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.group_item, null);
            }
            string textGroup = (string)GetGroup(groupPosition);
            TextView textViewGroup = convertView.FindViewById<TextView>(Resource.Id.group);
            textViewGroup.Text = textGroup;
            return convertView;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView==null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.item_layout, null);
            }
            TextView textViewItem = convertView.FindViewById<TextView>(Resource.Id.item);
            string content = (string)GetChild(groupPosition, childPosition);
            textViewItem.Text = content;
            return convertView;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new List<string>();
            lstChild.TryGetValue(listGroup[groupPosition], out result);
            return result.Count;
        }

        public override int GroupCount
        {
            get
            {
                return listGroup.Count;
            }
        }

        private void GetChildViewHelper(int groupPosition, int childPosition, out string Value)
        {
            char letter = (char)(65 + groupPosition);
            List<string> results = DataList.FindAll((string obj) => obj.Equals(letter));
            Value = results[childPosition];
        }

        #region implemented abstract members of BaseExpandableListAdapter

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            lstChild.TryGetValue(listGroup[groupPosition], out result);
            return result[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return listGroup[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            DataSota.chooseTrainer=(string)GetChild(groupPosition, childPosition);
            return true;
        }

        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}