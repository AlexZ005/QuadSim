using UnityEngine;
using System;
using System.IO;
using System.Collections;


public class Sample{
	public float health,knife,gun,enemy;
	public float[] output = new float[4]; 
	
	public Sample(string[] atts){								//переменные для загрузки цифр с файла
		health = Convert.ToSingle(atts[0]);
		knife = Convert.ToSingle(atts[1]);
		gun = Convert.ToSingle(atts[2]);
		enemy = Convert.ToSingle(atts[3]);
		output[0] = Convert.ToSingle(atts[4]);
		output[1] = Convert.ToSingle(atts[5]);
		output[2] = Convert.ToSingle(atts[6]);
		output[3] = Convert.ToSingle(atts[7]);
	}

}

public class Backpropagation : MonoBehaviour {
	
	//public int InputNeurons = 4;
	public int InputNeurons = 4;									//Входящие нейроны
	public int OutputNeurons = 4;
	public int HiddenNeurons2 = 30;		//default 3
	public float LearnRate = 0.7f;		//default 0.2
	private int RecordCount = 0;
	public GameObject player,npc;
	
	private float[,] weightIH,weightHO;
	private float[] inputs,hiddens,outputs,targets;
	private float[] erro,errh;
	private Sample[] samples = new Sample[7];//[19002]; //557/// Should be 7 as 0-7 columns, reads all lines
	
	private string[] commands = new string[4]{"A","B","C","D"};
	
	void Awake() {
	
		inputs = new float[InputNeurons];
		hiddens = new float[HiddenNeurons2];
		outputs = new float[OutputNeurons];
		targets = new float[OutputNeurons];
		
		erro = new float[OutputNeurons];
		errh = new float[HiddenNeurons2];
		
		weightIH = new float[InputNeurons+1,HiddenNeurons2];
		weightHO = new float[HiddenNeurons2+1,OutputNeurons];
		assignRandomWeights();

	}
	
	
	
	// Use this for initialization
	void Start () {
		//training();			//moved to start training by pressing T in quadPhysics!
		//test ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	
	public int HiddenProof() {
	return HiddenNeurons2;	//for quadPhysics GUI
	}
	
	
	/*
	 *  sigmoid()
	 *
	 *  Calculate and return the sigmoid of the val argument.
	 *
	 */
	float sigmoid(double val){
		return (float)((Mathf.Exp((float)2.0 * (float)val) - 1) / (Mathf.Exp((float)2.0 * (float)val) + 1)); //default! return (float)(1.0/(1.0+Mathf.Exp((float)-val))); //managed1 return (float)((2.0/(1.0+Mathf.Exp((float)-val)))-1.0);
	}
	
	/*
	 *  sigmoidDerivative()
	 *
	 *  Calculate and return the derivative of the sigmoid for the val argument.
	 *
	 */
	float sigmoidDerivative(double val){
		return (float)(1-((float)val*(float)val)); //default! return (float)(val*(1.0 - val));//managed1		return (float)((1/2)*(1.0+val)*(1.0 - val));				//производная
	}
	
	/*
	 *  feedForward()
	 *
	 *  Feedforward the inputs of the neural network to the outputs.
	 *
	 */
	void feedForward(){
		int i,h,o;
		double sum;
		 /* Calculate input to hidden layer */
		for (h = 0; h < HiddenNeurons2; h++){
			sum = 0.0;
			for (i = 0; i< InputNeurons; i++)
				sum += inputs[i]*weightIH[i,h];				//Для каждого элемента скрытого слоя vj находится входной сигнал 
			//Debug.Log(hiddens[h]);
			/* Add in Bias */
			sum += weightIH[InputNeurons,h];
			hiddens[h] = sigmoid(sum);
		}
		/* Calculate the hidden to output layer */
		for (o = 0; o < OutputNeurons; o++){
			sum = 0.0;
			for (h =0; h < HiddenNeurons2; h++)
				sum += hiddens[h] * weightHO[h,o];		//Для каждого элемента скрытого слоя yk находится входной сигнал 
			
			/* Add in Bias */
			sum += weightHO[HiddenNeurons2,o];
			outputs[o] = sigmoid(sum);
		}		
		
	}

		public float[] feedForwardContinue(float a, float b, float c, float d){							//feedForward для получения выходных параметров, которые потом применяются для квадкоптера
		int i,h,o;
		
		float[] inin = new float[4]; 
		inin[0]=a;
		inin[1]=b;
		inin[2]=c;
		inin[3]=d;
		
		double sum;
		 /* Calculate input to hidden layer */
		for (h = 0; h < HiddenNeurons2; h++){
			sum = 0.0;
			for (i = 0; i< InputNeurons; i++)
				sum += inin[i]*weightIH[i,h];				//Для каждого элемента скрытого слоя vj находится входной сигнал 
			//Debug.Log(hiddens[h]);
			/* Add in Bias */
			sum += weightIH[InputNeurons,h];
			hiddens[h] = sigmoid(sum);
		}
		/* Calculate the hidden to output layer */
		for (o = 0; o < OutputNeurons; o++){
			sum = 0.0;
			for (h =0; h < HiddenNeurons2; h++)
				sum += hiddens[h] * weightHO[h,o];		//Для каждого элемента скрытого слоя yk находится входной сигнал 
			
			/* Add in Bias */
			sum += weightHO[HiddenNeurons2,o];
			outputs[o] = sigmoid(sum);
		}
			return outputs;
			
	}
	
	
	public void SaveWeights() {
	
	//private float[,] weightIH,weightHO;
	var  fileName = "Assets/SavedData/BackP-2000-" + HiddenNeurons2 + "neurons.txt";
	
	if (File.Exists(fileName))
        {
            Debug.Log(fileName+" already exists.");
            return;
        }
        var sr = File.CreateText(fileName);
        
		
		/* for (h = 0; h < HiddenNeurons2; h++)
			for (i = 0; i< InputNeurons; i++)
			sr.WriteLine (weightIH[i,h]);
		Write the hidden to output layer
		for (o = 0; o < OutputNeurons; o++)
			for (h =0; h < HiddenNeurons2; h++)
				sr.WriteLine (weightHO[h,o]); */
		
		
/* 		for (int i = 0; i < InputNeurons; i++)
			for (int h = 0; h < HiddenNeurons2; h++)
				sr.WriteLine (weightIH[i,h]);
		
		for (int h=0; h < HiddenNeurons2; h++)
			for (int o = 0; o < OutputNeurons; o++)
				sr.WriteLine (weightHO[h,o]); */
		 
		int i,h,o;
		double sum;
		 /* Calculate input to hidden layer */
		for (h = 0; h < HiddenNeurons2; h++){
			for (i = 0; i< InputNeurons; i++)
				sr.WriteLine(weightIH[i,h]);				//Для каждого элемента скрытого слоя vj находится входной сигнал 
			//Debug.Log(hiddens[h]);
			/* Add in Bias */
			sr.WriteLine(weightIH[InputNeurons,h]);
		}
		/* Calculate the hidden to output layer */
		for (o = 0; o < OutputNeurons; o++){
			sum = 0.0;
			for (h =0; h < HiddenNeurons2; h++)
				sr.WriteLine(weightHO[h,o]);		//Для каждого элемента скрытого слоя yk находится входной сигнал 
			
			/* Add in Bias */
			sr.WriteLine(weightHO[HiddenNeurons2,o]);
		}

		 
		//Debug.Log("Numbers" + weightIH.Length);

	
//		 File.WriteAllLines(fileName, weightIH[,]);
		 
		//sr.WriteLine ("This is my file.");
        //sr.WriteLine ("I can write ints {0} or floats {1}, and so on.",
		//1, 4.2);
        sr.Close();
	
	}
	
	
	public void LoadWeights() {
	
			
	
	StreamReader readweights = (new FileInfo("Assets/SavedData/BackP-2000-" + HiddenNeurons2 + "neurons.txt")).OpenText();
		
		//Read the training dataset
		//string text = readweights.ReadLine();

/* 		
			int i,h,o;
		for (h = 0; h < HiddenNeurons2; h++)
			for (i = 0; i< InputNeurons; i++)
			{
			weightIH[i,h] = float.Parse(readweights.ReadLine());
			//Debug.Log("Read " + i + " " + h + " " + weightIH[i,h]);
			}
		// Write the hidden to output layer 
		for (o = 0; o < OutputNeurons; o++)
			for (h =0; h < HiddenNeurons2; h++)
			weightHO[h,o] = float.Parse(readweights.ReadLine());
	
 */	
	
			int i,h,o;
		double sum;
		 /* Calculate input to hidden layer */
		for (h = 0; h < HiddenNeurons2; h++){
			for (i = 0; i< InputNeurons; i++)
				weightIH[i,h]=float.Parse(readweights.ReadLine());				//Для каждого элемента скрытого слоя vj находится входной сигнал 
			//Debug.Log(hiddens[h]);
			/* Add in Bias */
			weightIH[InputNeurons,h]=float.Parse(readweights.ReadLine());
		}
		/* Calculate the hidden to output layer */
		for (o = 0; o < OutputNeurons; o++){
			sum = 0.0;
			for (h =0; h < HiddenNeurons2; h++)
				weightHO[h,o]=float.Parse(readweights.ReadLine());		//Для каждого элемента скрытого слоя yk находится входной сигнал 
			
			/* Add in Bias */
			weightHO[HiddenNeurons2,o]=float.Parse(readweights.ReadLine());
		}
	
		readweights.Close();
	
	
	}
	
	/*
	 *  backPropagate()
	 *
	 *  Backpropagate the error through the network.
	 *
	 */	
	void backPropagate(){
		int h,i,o;
		/* Calculate the output layer error (step 3 for output cell) */
		for (o =0; o < OutputNeurons; o ++)
			erro[o] = (targets[o]-outputs[o])*sigmoidDerivative(outputs[o]);		// Каждый выходной элемент Yl получает образец правильного ответа 
																											// dl и вычисляется корректирующий коэффициент
		/* Calculate the hidden layer error (step 3 for hidden cell) */
		for (h = 0; h < HiddenNeurons2; h++){
			errh[h]= 0.0f;
			for ( o = 0; o<OutputNeurons; o++)
				errh[h] += erro[o]*weightHO[h,o];												// Для каждого скрытого элемента vj 
																											// подсчитываем корректирующий входной коэффициент		
			errh[h] *= sigmoidDerivative(hiddens[h]);
		}
		
		/* Update the weights for the output layer (step 4 for output cell) */
		for ( o=0; o<OutputNeurons; o++){
			for ( h=0; h<HiddenNeurons2; h++)
				weightHO[h,o] += (LearnRate*erro[o]*hiddens[h]);
			/* Update the Bias */
			weightHO[HiddenNeurons2,o] += (LearnRate*erro[o]);
		}
		
		/* Update the weights for the hidden layer (step 4 for hidden cell) */
		for (h=0; h<HiddenNeurons2; h++){
			for (i=0; i<InputNeurons; i++)
				weightIH[i,h] += (LearnRate*errh[h]*inputs[i]);
			/* Update the Bias */
			weightIH[InputNeurons,h] += (LearnRate*errh[h]);
		}
		
	}
	
	void test(){
		int sum = 0;
		for (int i=0; i < RecordCount; i++){
			inputs[0] = samples[i].health;
			inputs[1] = samples[i].knife;
			inputs[2] = samples[i].gun;
			inputs[3] = samples[i].enemy;
			
					
			targets[0] = samples[i].output[0];
			targets[1] = samples[i].output[1];
			targets[2] = samples[i].output[2];
			targets[3] = samples[i].output[3];
			
			feedForward();
			if (action (outputs) == action(targets))
				sum++;
			
		}
		//Debug.Log("Result:"+(sum*100.0/RecordCount));
	}
	
	/*
	 *  training()
	 * 
	 *  Read training dataset and training the weight of networks.
	 * 
	 */
	public void training(){		//changed this method to public to be able to access from quadPhysics
		StreamReader reader = (new FileInfo("Assets/sample-data-2000.txt")).OpenText();
		
		//Read the training dataset
		string text = reader.ReadLine();

		int i = 0;
		int iterations_internal=0;
		while(text!=null){
			string[] result = text.Split(new char[]{','});
			//Debug.Log (result[0] + " " + result[1] + " " + result[2] + " " + result[3] + " " + result[4] + " " + result[5] + " " + result[6] + " " + result[7]);
			samples[i++] = new Sample(result);
			text = reader.ReadLine();
			//Debug.Log (text);
			
			
			float err = 0.0f;
			//
			//err = 0.0f;
			int iterations=0;
			
			do {
			
			
			inputs[0] = Convert.ToSingle(result[0]);
			inputs[1] = Convert.ToSingle(result[1]);
			inputs[2] = Convert.ToSingle(result[2]);
			inputs[3] = Convert.ToSingle(result[3]);
			
			// inputs[0] = -0.70660275220871f;
			// inputs[1] = 0.0294252708554268f;
			// inputs[2] = 0.0294480379670858f;
			// inputs[3] = 0.706384837627411f;
			
			// if (iterations_internal <= 0) //checked, inputs are correct!
			// {
			// ConsoleLog.Instance.Log("\nInputs\n[0] " + inputs[0]) ;
			// ConsoleLog.Instance.Log("[1] " + inputs[1]);
			// ConsoleLog.Instance.Log("[2] " + inputs[2]);
			// ConsoleLog.Instance.Log("[3] " + inputs[3]);
			// }
			
			targets[0] = Convert.ToSingle(result[4]);
			targets[1] = Convert.ToSingle(result[5]);
			targets[2] = Convert.ToSingle(result[6]);
			targets[3] = Convert.ToSingle(result[7]);

			// targets[0] = -0.000101594996452332f;
			// targets[1] = 0.000101483106613159f;
			// targets[2] = -0.00010159364938736f;
			// targets[3] = 0.000101705539226532f;
			
			
			feedForward();
			
			for (int j=0; j<OutputNeurons; j++)
				err += Mathf.Pow(targets[j]-outputs[j],2);
			
			Debug.Log("targ: " + targets[0] + " out: " + outputs[0]);	//targ: -0.02626049 out: 0.2640465
			
			err = (float)0.5*err;
			
			backPropagate();
			i=0;
			
			iterations++;
			iterations_internal++;
			Debug.Log(iterations_internal + " " + iterations + " mse="+err);
			if (iterations++ > 200)
				break;
			} while(err > 0.050);
			

			Debug.Log(iterations_internal + " mse="+err);
			ConsoleLog.Instance.Log("\nError " + err);
			
			
			
		}
		reader.Close();
		RecordCount = i;
		i = -1;
		
//		int iterations=0,MaxSamples=RecordCount;
		
		// while(true){
			// if (++i == MaxSamples)
				// i = 0;
			// //Debug.Log(iterations + " " + i + " " + samples[i].health);
			// inputs[0] = samples[i].health;
			// inputs[1] = samples[i].knife;
			// inputs[2] = samples[i].gun;
			// inputs[3] = samples[i].enemy;
			
			// targets[0] = samples[i].output[0];
			// targets[1] = samples[i].output[1];
			// targets[2] = samples[i].output[2];
			// targets[3] = samples[i].output[3];

			// feedForward();
			
			// float err = 0.0f;
			// for (int j=0; j<OutputNeurons; j++)
				// err += Mathf.Pow(samples[i].output[j]-outputs[j],2);
			
			// err = (float)0.5*err;
			// if (err < 2)							//wait until error is minimal!
			// break;
			// Debug.Log("mse="+err);
			// if (iterations++ > 200)
			// {
				// Debug.Log("Weights calculated for the sample-data");
				// break;
			// }
			// backPropagate();
		// }		
	}
	

	void assignRandomWeights(){									//Присваивание случайных весов
		for (int i = 0; i < InputNeurons+1; i++)
			for (int h = 0; h < HiddenNeurons2; h++)
			{
				//Debug.Log(HiddenNeurons2);
				weightIH[i,h] = UnityEngine.Random.Range(-1f, 1f);//UnityEngine.Random.value;
				//Debug.Log("Weight: i " + i + " h " + h + " value "+ weightIH[i,h]);
				}
		
		for (int h=0; h < HiddenNeurons2+1; h++)
			for (int o = 0; o < OutputNeurons; o++)
				weightHO[h,o] = UnityEngine.Random.Range(-1f, 1f);//UnityEngine.Random.value;
	}
	
	public string action(float[] vec){							//Функция вызываемая quadPhysics.cs
		inputs[0] = vec[0];
		inputs[1] = vec[1];
		inputs[2] = vec[2];
		//inputs[3] = vec[3];	
		
		targets[0] = -0.7f;
		targets[1] = 0.0f;
		targets[2] = 0.0f;
		targets[3] = 0.7f;
		
/* 		feedForward();
		
		int index = 0;
		
		double max = vec[index];
		
		for (int i=1; i<OutputNeurons; i++){
			if (vec[i]>max){
				max = vec[i];
				index = i;
			}
		} */
		
		
		int index = 0;
		int i = 0;
		RecordCount = 1;
		i = -1;
		
		int iterations=0,MaxSamples=RecordCount;
		
		while(true){
			if (++i == MaxSamples)
				i = 0;
			//Debug.Log(iterations + " " + i + " " + samples[i].health);
/* 			inputs[0] = vec[0];
			inputs[1] = vec[1];
			inputs[2] = vec[2];
			inputs[3] = vec[3];	 */
			//Debug.Log("inputs: [0] " + inputs[0]);
			

			//Debug.Log("targets: [0] " + targets[0]);
			
			feedForward();
			
			float err = 0.0f;
			for (int j=0; j<OutputNeurons; j++){
				err += Mathf.Pow(targets[j]-outputs[j],2);
				Debug.Log("NN output: " + outputs[j]);
				}
				//err += Mathf.Pow(samples[i].output[j]-outputs[j],2);
			//Debug.Log("err: " + err);
			
			
			err = (float)0.5*err;
			Debug.Log("mse="+err);
			//Debug.Log("mse="+err);
			if (iterations++ > 20)
				break;
			backPropagate();
			//Debug.Log("backPropagate inputs: [0] " + inputs[0] + " [1] " + inputs[1] + " [2] " + inputs[2] + " [3] " + inputs[3] );
		}
		/*
		foreach (float o in outputs)
			print(o);
		Debug.Log("Max:"+max);
		*/
		return (commands[index]);									//Возврат следующего действия
		//return (commands[index]);								//Возврат следующего действия
	}


}
