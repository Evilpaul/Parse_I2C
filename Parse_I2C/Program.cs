using System;
using System.Collections.Generic;
using System.IO;

namespace Parse_I2C
{
	class Program
	{
		static private string[] currString;
		static private Boolean in_transaction = false;
		static private string curr_transaction;
		static private string curr_start;
		static private string curr_dir;
		static private List<string> the_data = new List<string>();

		static void Main(string[] args)
		{
			// Check for correct arguments
			if (args.Length != 1)
			{
				Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " <input file>");
				return;
			}

			try
			{
				if (File.Exists(args[0]))
				{
					using (StreamReader sr = new StreamReader(args[0]))
					{
						Console.WriteLine("/* Data generated " + DateTime.Now.ToString("G") + " from \"" + args[0] + "\" */");

						while (sr.Peek() >= 0)
						{
							currString = sr.ReadLine().Trim().Split(',');
							if (currString.Length == 6)
							{
								if (in_transaction && (curr_transaction == currString[1]))
								{
									// we remain in a transaction, just add data
									the_data.Add(currString[3] + " " + currString[5]);
								}
								else if (in_transaction)
								{
									// start of new transaction, output current data and start again
									// Output the completed data
									Console.WriteLine("Transaction " + curr_transaction + ", Start Time: " + curr_start + ", Direction: " + curr_dir);
									foreach (string line in the_data)
									{
										Console.Write(line + ", ");
									}
									Console.WriteLine();
									Console.WriteLine();

									// wipe the lists
									the_data = new List<string>();

									// record new data
									curr_start = currString[0];
									curr_transaction = currString[1];
									curr_dir = currString[4];
									the_data.Add(currString[2]);
									the_data.Add(currString[3] + " " + currString[5]);
								}
								else
								{
									in_transaction = true;

									// first transaction
									curr_start = currString[0];
									curr_transaction = currString[1];
									curr_dir = currString[4];
									the_data.Add(currString[2]);
									the_data.Add(currString[3] + " " + currString[5]);
								}
							}
						}

						if(in_transaction)
						{
							// Output the completed data
							Console.WriteLine("Transaction " + curr_transaction + ", Start Time: " + curr_start + ", Direction: " + curr_dir);
							foreach (string line in the_data)
							{
								Console.Write(line + ", ");
							}
							Console.WriteLine();
							Console.WriteLine();
						}
					}
				}
				else
				{
					Console.WriteLine("File does not exist");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The process failed: {0}", e.ToString());
			}
		}
	}
}
