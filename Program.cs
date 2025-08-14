//.dmx is for double matrix files
//.json is for json files
//.fmx is for float matrix files
using System;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System.Text.Json;
using System.Dynamic;
using System.Diagnostics.CodeAnalysis;

namespace simulation_tools
{
    public record material_data(int ID, int radiation_range, double preservation_cost, double radiation_values_max,
        double radiation_values_min, Tuple<float,float, float> color_tuple);
    public record world_data(int length, int width, int number_of_materials, string global_path,
    int slot_index, material_limits materialLimits);
    public record material_limits(
        int radiation_range_min,
        int radiation_range_max,
        double preservation_cost_min,
        double preservation_cost_max,
        double radiation_values_min,
        double radiation_values_max

        );
    public class Material
    {
        public int ID { get; set; }
        public int radiation_range { get; set; }
        public double preservation_cost { get; set; }
        public Matrix<double> radiation_matrix { get; set; }
        public Tuple<float,float,float> color_tuple { get; set; }
        public string path { get; set; }
        public double radiation_values_max { get; set; }
        public double radiation_values_min { get; set; }
        public Material(int ID, int radiation_range, double preservation_cost, string path, double radiation_values_max,
        double radiation_values_min, Tuple<float,float,float> color_tuple)
        {
            this.ID = ID;
            //path is where the material will be stored in a directory of name material_ID
            this.path = path;
            this.radiation_range = radiation_range;
            this.preservation_cost = preservation_cost;
            this.radiation_values_max = radiation_values_max;
            this.radiation_values_min = radiation_values_min;
            this.radiation_matrix = Matrix<double>.Build.Dense(radiation_range, radiation_range);
            get_radiation_matrix(radiation_range, radiation_range);
            this.color_tuple = color_tuple;
        }
        public void get_radiation_matrix(int length, int width)
        {
            // Fill the matrix with some values (for example, random values)
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    radiation_matrix[i, j] = radiation_values_min + rand.NextDouble() * (radiation_values_max - radiation_values_min);
                }
            }
            int rad = radiation_range / 2;
            radiation_matrix[rad, rad] = -preservation_cost;
            Vector<double> absVec = radiation_matrix.ColumnAbsoluteSums();
            double absSum = absVec.Sum();
            radiation_matrix[rad, rad] = -absSum;

        }
        
    }
    public class World
    {
        public List<Material> Materials = new List<Material>();
        public int length { get; set; }
        public int width { get; set; }
        public Matrix<double> energy_matrix { get; set; }
        public Matrix<float> material_matrix { get; set; }
        public Matrix<double> energy_consumption_matrix { get; set; }
        public string global_path { get; set; }
        public int slot_index { get; set; }
        public string path { get; set; }
        public material_limits materialLimits { get; set; }
        public World(int length, int width, string global_path, int slot_index, material_limits materialLimits)
        {
            this.global_path = global_path;
            this.slot_index = slot_index;
            this.length = length;
            this.width = width;
            this.materialLimits = materialLimits;
            this.path = System.IO.Path.Combine(global_path, "slot_" + slot_index);
        }
        public World(int length, int width, int number_of_materials, string global_path,
         int slot_index, material_limits materialLimits)
        {
            this.global_path = global_path;
            this.slot_index = slot_index;
            this.length = length;
            this.width = width;
            this.materialLimits = materialLimits;
            this.energy_matrix = this.generate_energy_matrix();
            this.path = System.IO.Path.Combine(global_path, "slot_" + slot_index);
            generate_materials(number_of_materials);
            this.material_matrix = this.generate_material_matrix();
            this.energy_consumption_matrix = Matrix<double>.Build.Dense(length, width, 0.0);
            intialize_consumption_matrix();
        }
        public void AddMaterial(int radiation_range, double preservation_cost)
        {
            int ID = Materials.Count; // Generate a new ID based on the current count
            var rand = new MersenneTwister();
            string path = System.IO.Path.Combine(this.path, "world_data");
            Tuple<float, float, float> color_tuple = new Tuple<float, float, float>
            ((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
            Material newMaterial = new Material(ID, radiation_range, preservation_cost, path,
            materialLimits.radiation_values_max, materialLimits.radiation_values_min, color_tuple);
            Materials.Add(newMaterial);
        }
        public Matrix<double> generate_energy_matrix()
        {
            var normalDist = new Normal(0, 1000);
            energy_matrix = Matrix<double>.Build.Dense(length, width, (i, j) => normalDist.Sample());
            return energy_matrix;
        }
        public void generate_materials(int number_of_materials)
        {
            var rand = new Random();
            for (int i = 0; i < number_of_materials; i++)
            {
                int radiation_range = rand.Next(materialLimits.radiation_range_min, materialLimits.radiation_range_max);
                double min_cost = materialLimits.preservation_cost_min;
                double max_cost = materialLimits.preservation_cost_max;
                double preservation_cost = min_cost + rand.NextDouble() * (max_cost - min_cost);
                AddMaterial(radiation_range, preservation_cost);
            }
        }
        public Matrix<float> generate_material_matrix()
        {
            int rows = energy_matrix.RowCount;
            int cols = energy_matrix.ColumnCount;
            Matrix<float> material_matrix = Matrix<float>.Build.Dense(rows, cols, 0);
            var rand = new MersenneTwister();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Example logic to assign material IDs based on energy values
                    material_matrix[i, j] = (float)rand.Next(0, Materials.Count);
                }
            }
            return material_matrix;
        }
        public void update_energy_consumption_matrix(Tuple<int, int> position, int mode = 1)
        {
            int x = position.Item1;
            int y = position.Item2;
            Material material = Materials[(int)material_matrix[x, y]];
            energy_consumption_matrix[x, y] += mode * material.preservation_cost;
            int Ray = material.radiation_range / 2;
            for (int i = 0; i < material.radiation_range; i++)
            {
                for (int j = 0; j < material.radiation_range; j++)
                {
                    int newX = x - Ray + i;
                    int newY = y - Ray + j;
                    if (newX >= 0 && newX < length && newY >= 0 && newY < width)
                    {
                        energy_consumption_matrix[newX, newY] += mode * material.radiation_matrix[i, j];
                    }
                }
            }
        }
        public void intialize_consumption_matrix()
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    update_energy_consumption_matrix(new Tuple<int, int>(i, j), 1);
                }
            }
        }
        public List<Tuple<float, float, float>> get_all_colors()
        {
            List<Tuple<float, float, float>> colors = new List<Tuple<float, float, float>>();
            foreach (var material in Materials)
            {
                colors.Add(material.color_tuple);
            }
            return colors;
        }
        public void simulation()
        {
            // Implement the simulation logic here
            // This is a placeholder for the actual simulation logic
            energy_matrix += energy_consumption_matrix;
            List<Tuple<int, int>> defectPositions = get_defect_positions();
            var normalDist = new Normal(0, 1000);
            Random rand = new Random();
            foreach (var position in defectPositions)
            {
                int x = position.Item1;
                int y = position.Item2;
                update_energy_consumption_matrix(position, -1);
                int new_material_ID = rand.Next(0, Materials.Count);
                material_matrix[x, y] = (float)new_material_ID;
                update_energy_consumption_matrix(position, 1);
                energy_matrix[x, y] = normalDist.Sample();
            }
        }
        public List<Tuple<int, int>> get_defect_positions()
        {
            List<Tuple<int, int>> defectPositions = new List<Tuple<int, int>>();
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (energy_matrix[i, j] < 0)
                    {
                        defectPositions.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            return defectPositions;
        }
    }
    class matrix_data_manipulater
    {
        public void save_matrix(Matrix<double> matrix, string filePath)
        {
            int row = matrix.RowCount;
            int col = matrix.ColumnCount;
            double[] data = matrix.Storage.AsColumnMajorArray();
            using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream))
                {
                    writer.Write(row);
                    writer.Write(col);
                    Byte[] rowBytes = new byte[sizeof(double) * data.Length];
                    Buffer.BlockCopy(data, 0, rowBytes, 0, data.Length * sizeof(double));
                    writer.Write(rowBytes);
                }
            }

        }
        public void save_matrix(Matrix<float> matrix, string filePath)
        {
            int row = matrix.RowCount;
            int col = matrix.ColumnCount;
            float[] data = matrix.Storage.AsColumnMajorArray();
            using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream))
                {
                    writer.Write(row);
                    writer.Write(col);
                    Byte[] rowBytes = new byte[sizeof(float) * data.Length];
                    Buffer.BlockCopy(data, 0, rowBytes, 0, data.Length * sizeof(float));
                    writer.Write(rowBytes);
                }
            }
        }
        public Matrix<double> load_matrix_double(string filePath)
        {
            Matrix<double> matrix = Matrix<double>.Build.Dense(1, 1, 0.0);
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream))
                    {
                        int row = reader.ReadInt32();
                        int col = reader.ReadInt32();
                        double[] data = new double[row * col];
                        Byte[] rowBytes = reader.ReadBytes(sizeof(double) * data.Length);
                        Buffer.BlockCopy(rowBytes, 0, data, 0, rowBytes.Length);
                        matrix = Matrix<double>.Build.Dense(row, col, data);
                    }
                }
            }
            else
            {
                throw new System.IO.FileNotFoundException($"The file {filePath} does not exist.");
            }
            return matrix;
        }
        public Matrix<float> load_matrix_float(string filePath)
        {
            Matrix<float> matrix = Matrix<float>.Build.Dense(1, 1, 0.0f);
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream))
                    {
                        int row = reader.ReadInt32();
                        int col = reader.ReadInt32();
                        float[] data = new float[row * col];
                        Byte[] rowBytes = reader.ReadBytes(sizeof(float) * data.Length);
                        Buffer.BlockCopy(rowBytes, 0, data, 0, rowBytes.Length);
                        matrix = Matrix<float>.Build.Dense(row, col, data);
                    }
                }
            }
            else
            {
                throw new System.IO.FileNotFoundException($"The file {filePath} does not exist.");
            }
            return matrix;
        }
    }
    class material_data_manipulater
    {
        private string Path { get; set; }
        public material_data_manipulater(string path)
        {
            Path = path;
        }
        public void save_material(Material material, string directoryName)
        {
            string filePath = System.IO.Path.Combine(Path, directoryName);
            System.IO.Directory.CreateDirectory(filePath); // Ensure the directory exists
                                                           // Serialize material data to JSON
            material_data materialData = new material_data(
                material.ID,
                material.radiation_range,
                material.preservation_cost,
                material.radiation_values_max,
                material.radiation_values_min,
                material.color_tuple
            );
            string json = JsonSerializer.Serialize(materialData);
            System.IO.File.WriteAllText(System.IO.Path.Combine(filePath, "general_data.json"), json);
            matrix_data_manipulater matrixManipulator = new matrix_data_manipulater();
            string radiation_matrix_path = System.IO.Path.Combine(filePath, "radiation_matrix.dmx");
            matrixManipulator.save_matrix(material.radiation_matrix, radiation_matrix_path);
        }
        public Material load_material(string directoryName)
        {
            string filePath = System.IO.Path.Combine(Path, directoryName);
            filePath = System.IO.Path.Combine(filePath, "general_data.json");
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"The file {directoryName} does not exist at the specified path: {Path}");
            }
            string json = System.IO.File.ReadAllText(filePath);
            var materialData = JsonSerializer.Deserialize<material_data>(json);
            if (materialData == null)
            {
                throw new InvalidOperationException("Failed to deserialize material data.");
            }
            int ID = materialData.ID;
            int radiation_range = materialData.radiation_range;
            double preservation_cost = materialData.preservation_cost;
            double radiation_values_max = materialData.radiation_values_max;
            double radiation_values_min = materialData.radiation_values_min;
            var color_tuple = materialData.color_tuple;
            filePath = System.IO.Path.Combine(Path, directoryName);
            matrix_data_manipulater matrixManipulator = new matrix_data_manipulater();
            string radiation_matrix_path = System.IO.Path.Combine(filePath, "radiation_matrix.dmx");
            Matrix<double> radiation_matrix = matrixManipulator.load_matrix_double(radiation_matrix_path);
            Material material = new Material(ID, radiation_range, preservation_cost, Path, radiation_values_max,
                radiation_values_min, color_tuple);
            material.radiation_matrix = radiation_matrix;
            return material;
        }
    }
    class world_data_manipulater
    {
        private string Path { get; set; }
        public world_data_manipulater(string path)
        {
            Path = path;
        }
        public void save_world(World world, string fileName)
        {
            string filePath = System.IO.Path.Combine(Path, fileName);
            System.IO.Directory.CreateDirectory(filePath); // Ensure the directory existsS
            matrix_data_manipulater matrixManipulator = new matrix_data_manipulater();
            string world_energy_martix_path = System.IO.Path.Combine(filePath, "energy_matrix.dmx");
            matrixManipulator.save_matrix(world.energy_matrix, world_energy_martix_path);
            string world_material_matrix_path = System.IO.Path.Combine(filePath, "material_matrix.fmx");
            matrixManipulator.save_matrix(world.material_matrix, world_material_matrix_path);
            string world_energy_consumption_matrix_path = System.IO.Path.Combine(filePath, "energy_consumption_matrix.dmx");
            matrixManipulator.save_matrix(world.energy_consumption_matrix, world_energy_consumption_matrix_path);
            material_data_manipulater materialManipulator = new material_data_manipulater(filePath);
            foreach (var material in world.Materials)
            {
                string materialFileName = $"material_{material.ID}";
                materialManipulator.save_material(material, materialFileName);
            }
            world_data worldData = new world_data(
                world.length,
                world.width,
                world.Materials.Count,
                world.global_path,
                world.slot_index,
                world.materialLimits
            );
            string worldDataPath = System.IO.Path.Combine(filePath, "general_data.json");
            string json = JsonSerializer.Serialize(worldData);
            System.IO.File.WriteAllText(worldDataPath, json);
        }
        public World load_world(string fileName)
        {
            string filePath = System.IO.Path.Combine(Path, fileName);
            if (!System.IO.Directory.Exists(filePath))
            {
                throw new System.IO.DirectoryNotFoundException($"The directory {fileName} does not exist at the specified path: {Path}");
            }
            matrix_data_manipulater matrixManipulator = new matrix_data_manipulater();
            string world_energy_martix_path = System.IO.Path.Combine(filePath, "energy_matrix.dmx");
            Matrix<double> energy_matrix = matrixManipulator.load_matrix_double(world_energy_martix_path);
            string world_material_matrix_path = System.IO.Path.Combine(filePath, "material_matrix.fmx");
            Matrix<float> material_matrix = matrixManipulator.load_matrix_float(world_material_matrix_path);
            string world_energy_consumption_matrix_path = System.IO.Path.Combine(filePath, "energy_consumption_matrix.dmx");
            Matrix<double> energy_consumption_matrix = matrixManipulator.load_matrix_double(world_energy_consumption_matrix_path);
            string worldDataPath = System.IO.Path.Combine(filePath, "general_data.json");
            if (!System.IO.File.Exists(worldDataPath))
            {
                throw new System.IO.FileNotFoundException($"The file {worldDataPath} does not exist.");
            }
            string json = System.IO.File.ReadAllText(worldDataPath);
            var worldData = JsonSerializer.Deserialize<world_data>(json);
            if (worldData == null)
            {
                throw new InvalidOperationException("Failed to deserialize world data.");
            }
            World world = new World(
                worldData.length,
                worldData.width,
                worldData.global_path,
                worldData.slot_index,
                worldData.materialLimits
            );
            world.energy_matrix = energy_matrix;
            world.material_matrix = material_matrix;
            world.energy_consumption_matrix = energy_consumption_matrix;
            material_data_manipulater materialManipulator = new material_data_manipulater(filePath);
            for (int i = 0; i < worldData.number_of_materials; i++)
            {
                string materialFileName = $"material_{i}";
                Material material = materialManipulator.load_material(materialFileName);
                world.Materials.Add(material);
            }
            world.path = Path;
            return world;
        }
    }
}