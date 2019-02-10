using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering {
    static class Tests {
        // Classification Separate Test
        public static void Perform1() {
            //data reading before data mining 
            Program example = new Program("../../aa.txt");
            IList<String> names = example.readNames("../../names.txt");
            IList<int> idList = example.readID("../../id.txt");
            IList<IList<Double>> clusters = example.clustering(6);
            int i = 0;
            //clusters initialization and sorting
            example.InitializeCategories(clusters);
            i = 0;
            int j = 0;
            int k = 0;
            example.initializeCategories();
            foreach (Vec2<IList<Double>, Double> vector in example.GetSortedClusters()) {
                // Console.WriteLine("Cluster " + (i++));
                Console.WriteLine(example.GetCategories()[i++]);
                foreach (Double d in vector.getTValue()) {
                    Console.Write("ID: " + idList[k++] + "|");
                    Console.Write(names[j++] + ": ");
                    Console.WriteLine(d);
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }

        // Evaluation Test
        public static void Perform2() {
            Evaluation metrics = new Evaluation("../../dataset.txt");
            metrics.GetAPrioriEntrophy();
            metrics.GetAPosterioriEntophy();
            metrics.GetPropertyGains();
            metrics.RNN_Compute_h();
            metrics.RNN_Compute_o();
            int sosdl = 2;
        }

        // Main Method
        static void Main(string[] args) {
            Perform1();
            Perform2();
        }
    } //end Tests
} //end Clustering
