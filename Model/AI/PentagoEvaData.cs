using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PentagoWeb.Model.AI
{
    public class PentagoEvaData
    {
        public static readonly int[,][] LineTable ={
            {new int[]{1,13,25},new int[]{2,13,19,29},new int[]{3,13,19},new int[]{4,13,19},new int[]{5,13,19,32},new int[]{6,19,28}},
            {new int[]{1,7,14,30},new int[]{2,8,14,20,25,26},new int[]{3,9,14,20,29},new int[]{4,10,14,20,32},new int[]{5,11,14,20,27,28},new int[]{6,12,20,31}},
            {new int[]{1,7,15},new int[]{2,8,15,21,30},new int[]{3,9,15,21,25,26,32},new int[]{4,10,15,21,27,28,29},new int[]{5,11,15,21,31},new int[]{6,12,21}},
            {new int[]{1,7,16},new int[]{2,8,16,22,32},new int[]{3,9,16,22,27,28,30},new int[]{4,10,16,22,25,26,31},new int[]{5,11,16,22,29},new int[]{6,12,22}},
            {new int[]{1,7,17,32},new int[]{2,8,17,23,27,28},new int[]{3,9,17,23,31},new int[]{4,10,17,23,30},new int[]{5,11,17,23,25,26},new int[]{6,12,23,29}},
            {new int[]{7,18,27},new int[]{8,18,24,31},new int[]{9,18,24},new int[]{10,18,24},new int[]{11,18,24,30},new int[]{12,24,26}}
        };
        public static readonly int[] Values = { 0, 1, 3, 9, 50, 1000 };
    }
}
