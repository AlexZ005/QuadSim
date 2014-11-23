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
	public int InputNeurons = 3;									//Входящие нейроны
	public int OutputNeurons = 4;
	public int HiddenNeurons = 3;
	public float LearnRate = 0.2f;
	private int RecordCount = 0;
	public GameObject player,npc;
	
	private float[,] weightIH,weightHO;
	private float[] inputs,hiddens,outputs,targets;
	private float[] erro,errh;
	private Sample[] samples = new Sample[50];
	
	private string[] commands = new string[4]{"A","B","C","D"};
	
	void Awake() {
		inputs = new float[InputNeurons];
		hiddens = new float[HiddenNeurons];
		outputs = new float[OutputNeurons];
		targets = new float[OutputNeurons];
		
		erro = new float[OutputNeurons];
		errh = new float[HiddenNeurons];
		
		weightIH = new float[InputNeurons+1,HiddenNeurons];
		weightHO = new float[HiddenNeurons+1,OutputNeurons];
		assignRandomWeights();

	}
	
	// Use this for initialization
	void Start () {
		tranning();		
		test ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	/*
	 *  sigmoid()
	 *
	 *  Calculate and return the sigmoid of the val argument.
	 *
	 */
	float sigmoid(double val){
		return (float)((2.0/(1.0+Mathf.Exp((float)-val)))-1);
	}
	
	/*
	 *  sigmoidDerivative()
	 *
	 *  Calculate and return the derivative of the sigmoid for the val argument.
	 *
	 */
	float sigmoidDerivative(double val){
		return (float)((1/2)*(1+val)*(1.0 - val));				//производная
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
		for (h = 0; h < HiddenNeurons; h++){
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
			for (h =0; h < HiddenNeurons; h++)
				sum += hiddens[h] * weightHO[h,o];		//Для каждого элемента скрытого слоя yk находится входной сигнал 
			
			/* Add in Bias */
			sum += weightHO[HiddenNeurons,o];
			outputs[o] = sigmoid(sum);
		}		
		
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
		for (h = 0; h < HiddenNeurons; h++){
			errh[h]= 0.0f;
			for ( o = 0; o<OutputNeurons; o++)
				errh[h] += erro[o]*weightHO[h,o];												// Для каждого скрытого элемента vj 
																											// подсчитываем корректирующий входной коэффициент		
			errh[h] *= sigmoidDerivative(hiddens[h]);
		}
		
		/* Update the weights for the output layer (step 4 for output cell) */
		for ( o=0; o<OutputNeurons; o++){
			for ( h=0; h<HiddenNeurons; h++)
				weightHO[h,o] += (LearnRate*erro[o]*hiddens[h]);
			/* Update the Bias */
			weightHO[HiddenNeurons,o] += (LearnRate*erro[o]);
		}
		
		/* Update the weights for the hidden layer (step 4 for hidden cell) */
		for (h=0; h<HiddenNeurons; h++){
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
	void tranning(){
		StreamReader reader = (new FileInfo("Assets/TrainingDataSet.txt")).OpenText();
		
		//Read the training dataset
		string text = reader.ReadLine();

		int i = 0;
		while(text!=null){
			string[] result = text.Split(new char[]{','});
			samples[i++] = new Sample(result);
			text = reader.ReadLine();
			//Debug.Log (text);

		}
		reader.Close();
		RecordCount = i;
		i = -1;
		
		int iterations=0,MaxSamples=RecordCount;
		
		while(true){
			if (++i == MaxSamples)
				i = 0;
			//Debug.Log(iterations + " " + i + " " + samples[i].health);
			inputs[0] = samples[i].health;
			inputs[1] = samples[i].knife;
			inputs[2] = samples[i].gun;
			inputs[3] = samples[i].enemy;
			
			targets[0] = samples[i].output[0];
			targets[1] = samples[i].output[1];
			targets[2] = samples[i].output[2];
			targets[3] = samples[i].output[3];

			feedForward();
			
			float err = 0.0f;
			for (int j=0; j<OutputNeurons; j++)
				err += Mathf.Pow(samples[i].output[j]-outputs[j],2);
			
			err = (float)0.5*err;
			//Debug.Log("mse="+err);
			if (iterations++ > 200)
				break;
			backPropagate();
		}		
	}
	

	void assignRandomWeights(){									//Присваивание случайных весов
		for (int i = 0; i < InputNeurons+1; i++)
			for (int h = 0; h < HiddenNeurons; h++)
				weightIH[i,h] = UnityEngine.Random.value;
		
		for (int h=0; h < HiddenNeurons+1; h++)
			for (int o = 0; o < OutputNeurons; o++)
				weightHO[h,o] = UnityEngine.Random.value;
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
			

			Debug.Log("targets: [0] " + targets[0]);
			
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
