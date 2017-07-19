using System;


//This class will be our Experiment Singleton system for the goal of cleaner code!!!
public class ExperimentSingleton
{

	//Classic singleton design pattern XD
	private static ExperimentSingleton _exp;

	public static ExperimentSingleton GetInstance()
	{
		return _exp ?? (_exp = new ExperimentSingleton());
	}


	private ExperimentSingleton ()
	{
	}
}


