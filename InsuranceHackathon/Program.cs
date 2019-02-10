using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    /*class program implements the 
     * clustering using the main part of Expectation Maximization
     * algorithm based on income*/
    class Program {
        //this list contains the data 
        IList<Double> dataset;
        //this list contains the names of the insured ppl
        IList<String> names;
        //this list consists of the ids of the customers
        IList<int> id;
        //this list contains the clusters after sorting
        List<Vec2<IList<Double>, Double>> sortedClusters;
        //this list contains the categories
        IList<String> categories;

        //constructor
        public Program(String path)  {
            IList<Double> dataset = new List<Double>();
            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i += 1) {
                Double line = Convert.ToDouble(lines[i]);
                dataset.Add(line);
            }
            //outputDataset(dataset);
            this.dataset = dataset;
        }

        /* public Program(IList<Double> values,Dictionary<String,Double> dicts)
         {
             dataset = values;



             for(int i=0; i < dicts.Count; i++)
             {

             }
         }*/

        //this method reads the names of the insured people -used for testing of the data mining algorithm NOT the connection with the nueral network
        public IList<String> readNames(String path)  {
            IList<String> names = new List<String>();
            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i += 1)   {
                String line = lines[i];
                names.Add(line);
                this.names = names;
            }
            return names;
        }

        //Read id of the insured ppl - used for testing of the data mining algorithm NOT for the connection with the neural network 
        public IList<int> readID(String path) {
            IList<int> id = new List<int>();
            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i += 1) {
                int line = Convert.ToInt16(lines[i]);
                id.Add(line);
                this.id = id;
            }
            return id;
        }

        //initializes the list which contains the insurance packages
        public void initializeCategories()  {
            categories = new List<String>();
            categories.Add("Rejected");
            categories.Add("Economy");
            categories.Add("Basic");
            categories.Add("Premium");
            categories.Add("Silver");
            categories.Add("Gold");
        }

        //printing the dataset
        public void outputDataset(IList<Double> list) {
            foreach (Double d in list) {
                Console.WriteLine(d);
            }
        }
        
        //calculate distance between items d1 and d2
        public Double getDistance(Double d1, Double d2)  {
            return Math.Abs(d1 - d2);
        }
        
        //calculate distance of an item to a cluster
        public Double getDistance(Double d1, IList<Double> cluster) {
            Double distance = 0;
            foreach (Double d2 in cluster) {
                distance += Math.Pow(getDistance(d1, d2), 2);
            }
            distance /= cluster.Count;
            return distance;
        }
       
        //finds for item , the nearest centroid among the candidates
        public IList<Double> findNearestCluster(Double d1, IList<IList<Double>> config)  {
            IList<Double> nearestCluster = config.ElementAt(0);
            Double minD = getDistance(d1, nearestCluster.ElementAt(0));
            foreach (IList<Double> candidateCluster in config)  {
                if ((candidateCluster != nearestCluster) && (candidateCluster.Count > 0)) {
                    Double d2 = getDistance(d1, candidateCluster.ElementAt(0));
                    if (d2 < minD)  {
                        minD = d2;
                        nearestCluster = candidateCluster;
                    }
                }
            }
            return nearestCluster;
        }

        //checks if an item is not placed in the nearest cluster
        public Boolean changeNeeded(IList<IList<Double>> config) {
            foreach (IList<Double> cluster in config) {
                foreach (Double value in cluster)  {
                    if (findNearestCluster(value, config) != cluster) return true;
                }
            }
            return false;
        }

        //update
        public int Update(IList<IList<Double>> config) {
            int changes = 0;
            foreach (IList<Double> cluster in config) {
                int offset = 0;
                Double centroid = cluster.ElementAt(offset);
                Double minD = getDistance(centroid, cluster);
                for (int i = 1; i < cluster.Count; i++)  {
                    Double candidate = cluster.ElementAt(i);
                    Double d = getDistance(candidate, cluster);
                    if (d < minD) {
                        minD = d;
                        centroid = candidate;
                        offset = i;
                        changes++;
                    }
                }
                if (offset != 0)  {
                    Double temp = cluster.ElementAt(0);
                    cluster[0] = cluster.ElementAt(offset);
                    cluster[offset] = temp;
                }
            }
            return changes;
        }

        //assignment
        public void assignment(IList<IList<Double>> config)  {
            IList<Double> dataset = new List<Double>();
            foreach (IList<Double> cluster in config) {
                Double centroid = cluster.ElementAt(0);
                for (int i = 1; i < cluster.Count; i++) {
                    dataset.Add(cluster.ElementAt(i));
                }
                cluster.Clear();
                cluster.Add(centroid);
            }
            foreach (Double value in dataset) {
                findNearestCluster(value, config).Add(value);
            }
        }
        //K-means implementation with Forgy initialization and update algorithm  
        public IList<IList<Double>> clustering(int c)  {
            IList<IList<Double>> config = new List<IList<Double>>();
            const int MAX_LOOPS = 100000;
            int n = 0;
            Random randomGenerator = new Random(1);
            int points = dataset.Count;
            if (c > dataset.Count) {
                throw new Exception("There are more clusters than data.");
            }
            for (int i = 0; i < c; i++) {
                config.Add(new List<Double>());
            }
            // Initialize clusters with Forgy method
            foreach (IList<Double> cluster in config)  {
                int p = randomGenerator.Next(0, dataset.Count);
                cluster.Add(dataset.ElementAt(p));
                dataset.RemoveAt(p);
            }
            if (c == points) return config;
            foreach (Double value in dataset) {
                IList<Double> cluster = findNearestCluster(value, config);
                cluster.Add(value);
            }
            for (int k = 1; k < MAX_LOOPS; k++) {
                int changes = Update(config);
                if (changes == 0) break;
                assignment(config);
            }
            return config;
        }

        //Categories list initialization
        public void InitializeCategories(IList<IList<Double>> clusters) {
            sortedClusters = new List<Vec2<IList<Double>, Double>>();
            foreach (IList<Double> inCluster in clusters) {
                double mean = 0;
                foreach (Double d in inCluster) mean += d;
                mean /= inCluster.Count;
                // Console.WriteLine("Cluster mean: " + mean);
                sortedClusters.Add(new Vec2<IList<Double>, Double>(inCluster, mean));
            }
            sortedClusters.Sort();
        }

        public List<Vec2<IList<Double>, Double>> GetSortedClusters() {
            return sortedClusters;
        }

        public IList<String> GetCategories() {
            return categories;
        }
    } //end program

    /*
     * insurance class represents the 
     * insured person.Fields: income,due,bonus
     * not used
     */
    class Insured {
        //lastname of the insured person
        public String lastname;
        //Insured person's Income and due. By due we mean the due to third party people
        public Double income, due;
        public int bonus, id;
        //constructor
        public Insured(String lastname, Double a, Double b, int c, int id)  {
            this.lastname = lastname;
            income = a;
            due = b;
            bonus = c;
            this.id = id;
        }
        //getters-setters
        public int getID
        {
            get; set;
        }
        public String getLastname
        {
            get; set;
        }
        public double getIncome
        {
            get; set;
        }
        public double getDue
        {
            get; set;
        }
        public int getBonus
        {
            get; set;
        }
        public String ToString()
        {
            return "Customer's id: " + id + " lastname: " + lastname + " Income: " + income + " Due: " + " Bonus:" + bonus;
        }
    }
} //end clustering
