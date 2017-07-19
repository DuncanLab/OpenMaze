using System;


//This class will be our Experiment Singleton system for the goal of cleaner code!!!
public class ExperimentSingleton
{

	//Classic singleton design pattern XD
	private static ExperimentSingleton exp;

	public static ExperimentSingleton getInstance(){
		if (exp == null) exp = new ExperimentSingleton();

		return exp;
	}


	private ExperimentSingleton ()
	{
	}
}


