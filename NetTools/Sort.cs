using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace NetTools
{
    public class Sort
    {
        public delegate int DelegateArrayListCompare(object obj1, object obj2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="compare"></param>
        /// <param name="desc"></param>
        /// <param name="keep_before_sort">종류별로 소팅할 때 이전의 소팅을 유지하기 위해서 예를 들어 태그명 정렬 후 종류별 정렬을 할때 종류별로 정렬하더라도 이름정렬이 기본적으로 유지된다. 사용하게 되면 속도는 약간 늦은것 같다</param>
        public static void ArrayListSort(ArrayList array, DelegateArrayListCompare compare, bool desc, bool keep_before_sort)
        {
            int i, j;
            
            object temp;

            if (desc)
            {
                int big;
                for (i = 0; i < array.Count; i++)
                {
                    big = i;
                    for (j = i + 1; j < array.Count; j++)
                    {
                        if (compare(array[j], array[big]) > 0) big = j;
                    }

                    if (big != i)
                    {
                        if (keep_before_sort)
                        {
                            temp = array[big];
                            array.RemoveAt(big);
                            array.Insert(i, temp);
                        }
                        else
                        {
                            temp = array[i];
                            array[i] = array[big];
                            array[big] = temp;
                        }
                    }
                }
            }
            else
            {
                int small;
                for (i = 0; i < array.Count; i++)
                {
                    small = i;
                    for (j = i + 1; j < array.Count; j++)
                    {
                        if (compare(array[j], array[small]) < 0) small = j;
                    }

                    if (small != i)
                    {
                        if (keep_before_sort)
                        {
                            temp = array[small];
                            array.RemoveAt(small);
                            array.Insert(i, temp);
                        }
                        else
                        {
                            temp = array[i];
                            array[i] = array[small];
                            array[small] = temp;
                        }
                    }
                }
            }
        }

        
    }
}
