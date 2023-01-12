# Machine-learning airplanes

**Welcome to my Machine-Learning Airplane Game!**

- In this game, you take control of an airplane and navigate through a challenging checkpoint course.
- You will compete against [machine-learned](https://en.wikipedia.org/wiki/Machine_learning) airplanes, which have been trained to navigate the course.
- With intuitive controls, you will guide your airplane through the course, trying to beat the best time set by the AI-controlled airplanes.
- Get ready to take flight and test your skills in this exciting game of aviation and AI competition!


# Check it out, it's AI-mazing!

![A demo gif of what the game looks like, you can still find the gif in the root of the repository](DemoAirplanesGithub.gif)

# So how does it work?

For this project i used Unity's ML-Agents to train my airplanes.

*So what is ML-Agents, and what does it do?*

ML-Agents is a Unity plugin that allows games and simulations to serve as environments for training intelligent agents. The plugin provides a set of pre-built agents and a set of Python scripts to train them using a state-of-the-art reinforcement learning algorithm called PPO (Proximal Policy Optimization).

To train an agent, you will need to provide a Unity scene that serves as the environment and a configuration file that specifies the details of the training process, such as the number of agents, the type of observations, and the type of actions. The scene must be set up to use the ML-Agents API, which provides a set of C# scripts to interact with the agents and to send and receive messages to the Python training scripts.

Once the scene and the configuration are set up, you can start the training process by running a python script. This script will launch the Unity environment, create the agents, and start the PPO algorithm. The algorithm will iteratively update the policy of the agents by adjusting the parameters of a neural network, based on the rewards and observations obtained during the interactions with the environment.

The training process can be monitored using TensorBoard, which is a tool that allows you to visualize the progress of the training and the performance of the agents.

You can also find pre-trained models in the Unity's ML-Agents GitHub repository, which you can use as a starting point to further train the model or adapt it to your specific use case.

# Observations

ML-agents require observations through sensors such as cameras, audio, lasers to understand and interact with their environment. They also use ray-casting to detect objects and obstacles for navigation and decision making. Without observations, ML-agents would not be able to function.
You can also add raw data for the observations.

The observations i used are:
- The agent's current linear velocity
- The position of the next checkpoint
- The forward vector of the next checkpoint
- Raycasts in the foward vector of the agent, 15 angles up and 15 angles down

*With That Information Only The Airplane Was Able To Learn The Whole Course*

# Rewards

In order for ML-Agents to learn and improve their performance, they rely on a system of rewards. Rewards are numerical values that the agent receives based on its actions and the state of the environment it's in. These rewards act as a feedback mechanism, allowing the agent to adjust its behavior in order to maximize the total reward received. Without rewards, the agent would have no way to evaluate its performance and make decisions on how to improve.

**But be careful!!!**

The way you implement your reward system can drastically change the way your AI's train.
So if you see your AI's aren't training well over time, it probably has something to do with the way you are rewarding them.

The rewards i used for my agents:
- -0.000000001 for living every step it lives (kinda cruel, but necesarry) otherwise the agent could just stand still
- -0.5  if the he is too slow before reaching the next checkpoint
- -1 if he collides with terrain
-   0.5 for reaching a checkpoint


------------

**Here are some graphs showing how the agents learned over a time of 1.5 hours: 
**
