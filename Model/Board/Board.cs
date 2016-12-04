using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace PentagoWeb.Model.Board
{
    public class Board<T>:AbstractBoard<T> where T:ICloneable<T>,IComparable<T>
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        private ObservableCollection<T> content;

        public override Collection<T> Content
        {
            get { return content; }
        }


        public Board(int width, int height, T defaultValue)
        {
            this.Width = width;
            this.Height = height;
            content = new ObservableCollection<T>();
            for (int i = 0; i < width * height; i++)
                content.Add(defaultValue.Clone());

            content.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(content_CollectionChanged);
        }

        protected Board(int width, int height, Collection<T> content)
        {
            this.Width = width;
            this.Height = height;
            this.content = new ObservableCollection<T>(content);
        }

        void content_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Debug.WriteLine("changed");
            //Debug.WriteLine(e.Action.ToString());
        }

        public override T this[int x, int y]
        {
            get
            {
                return content[y * Width + x];
            }
            set
            {
                content[y * Width + x] = value;

            }
        }

        public T this[int index]
        {
            get
            {
                return content[index];
            }
            set
            {
                content[index] = value;

            }
        }

        /// <summary>
        /// Get a copy of current board
        /// </summary>
        /// <returns>the clone</returns>
        public override AbstractBoard<T> Clone()
        {
            var clone = new Board<T>(this.Width, this.Height, this[0]);
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    clone[i, j] = this[i, j].Clone();
                }
            }
            return clone;
        }

        public override bool Equals(object obj)
        {
            if (obj is Board<T>)
            {
                var another = obj as Board<T>;
                if(this.Content.Count!= another.Content.Count)
                    return false;
                for (int i = 0; i < this.Content.Count;i++ )
                {
                    if (!this.Content[i].EqualsTo(another.Content[i]))
                        return false;
                }
                return true;
            }
            else
                return false;
        }


    }
}
