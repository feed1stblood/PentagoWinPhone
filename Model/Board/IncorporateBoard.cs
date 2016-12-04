using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PentagoWeb.Model.Board
{
    public class IncorporateBoard<K, T> : AbstractBoard<T>
        where T : ICloneable<T>, IComparable<T>
        where K : Board<T>
    {
        public int NumberOfSectionsPerRow { get; private set; }
        public ObservableCollection<K> Sections { get; private set; }
        public IncorporateBoard(IEnumerable<K> sections, int numberOfSectionsPerRow)
        {
            Sections = new ObservableCollection<K>(sections);
            this.NumberOfSectionsPerRow = numberOfSectionsPerRow;
        }



        public override T this[int x, int y]
        {
            get
            {
                if(!IsValidPosition(x,y))
                    return default(T);
                int sectionY = y / Sections[0].Height;
                int sectionX = x / Sections[0].Width;
                int remainderY = y % Sections[0].Height;
                int remainderX = x % Sections[0].Width;
                return Sections[sectionY * NumberOfSectionsPerRow + sectionX][remainderX, remainderY];
            }
            set
            {
                if (!IsValidPosition(x, y))
                    return;
                int sectionY = y / Sections[0].Height;
                int sectionX = x / Sections[0].Width;
                int remainderY = y % Sections[0].Height;
                int remainderX = x % Sections[0].Width;
                Sections[sectionY * NumberOfSectionsPerRow + sectionX][remainderX, remainderY] = value;
            }

        }

        public bool IsValidPosition(int x, int y)
        {
            if (x < 0)
                return false;
            if (x >= TotalWidth)
                return false;
            if (y < 0)
                return false;
            if (y >= TotalHeight)
                return false;
            return true;
        }

        public int TotalWidth { get { return this.NumberOfSectionsPerRow * Sections[0].Width; } }
        public int TotalHeight { get { return Sections.Count / this.NumberOfSectionsPerRow * Sections[0].Height; } }

        public bool IsFull(T empty)
        {
            return CountSteps(empty) == TotalWidth * TotalHeight;
        }

        public bool IsEmpty(T empty)
        {
            return CountSteps(empty) == 0;
        }

        public int CountSteps(T empty)
        {
            int count = 0;
            for (int i = 0; i < TotalWidth; i++)
                for (int j = 0; j < TotalHeight; j++)
                {
                    if (!CheckTEqualsMethod(this[i, j], empty))
                        count++;
                }
            return count;
        }


        public bool HasConsecutiveNPiece(T status, int n)
        {
            for (int i = 0; i < TotalWidth; i++)
                for (int j = 0; j < TotalHeight; j++)
                {
                    //check for horizontal case
                    if (i + n <= this.TotalWidth)
                    {
                        int k = 0;
                        while (k < n && CheckTEqualsMethod(this[i + k, j], status))
                            k++;
                        if (k == n)
                            return true;
                    }

                    //check for vertical case
                    if (j + n <= this.TotalWidth)
                    {
                        int k = 0;
                        while (k < n && CheckTEqualsMethod(this[i, j + k], status) && k < n)
                            k++;
                        if (k == n)
                            return true;
                    }

                    //check for diagonal case
                    if (i + n <= this.TotalWidth && j + n <= this.TotalWidth)
                    {
                        int k = 0;
                        while (k < n && CheckTEqualsMethod(this[i + k, j + k], status))
                            k++;
                        if (k == n)
                            return true;
                    }
                    if (i + n <= this.TotalWidth && j - n >= -1)
                    {
                        int k = 0;
                        while (k < n && CheckTEqualsMethod(this[i + k, j - k], status))
                            k++;
                        if (k == n)
                            return true;
                    }

                }
            return false;
        }

        public delegate bool CheckTEqualsDelegate(T obj1, T obj2);
        public CheckTEqualsDelegate CheckTEqualsMethod = new CheckTEqualsDelegate(CheckTEqual);
        static bool CheckTEqual(T obj1, T obj2)
        {
            return obj1.EqualsTo(obj2);
        }

        public override Collection<T> Content
        {
            get
            {
                var result = new List<T>();
                foreach (var section in Sections)
                {
                    result = result.Concat(section.Content).ToList();
                }
                return new Collection<T>(result);
            }
        }

        public override AbstractBoard<T> Clone()
        {
            Collection<K> contentClone = new Collection<K>();
            for (int i = 0; i < this.Sections.Count; i++)
            {
                contentClone.Add(this.Sections[i].Clone() as K);
            }
            return new IncorporateBoard<K, T>(contentClone, this.NumberOfSectionsPerRow);
        }
    }
}
