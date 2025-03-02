// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright � Tom Bogin, 2018
// https://github.com/mentaman
// 
// Copyright � C�sar Souza, 2009-2017
// cesarsouza at gmail.com
//
// Copyright � Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.MachineLearning
{
    using System;
    using Accord.Compat;
    using System.Collections.Generic;
    using System.Numerics;

    /// <summary>
    /// QLearning learning algorithm with infinite number of states.
    /// </summary>
    /// 
    /// <remarks>The class provides implementation of Q-Learning algorithm, known as
    /// off-policy Temporal Difference control.</remarks>
    /// 
    /// <seealso cref="QLearning"/>
    /// <seealso cref="Sarsa"/>
    /// 
    [Serializable]
    public class InfiniteQLearning
    {
        // amount of possible states
        private BigInteger states;

        // amount of possible actions
        private int actions;

        // q-values
        private Dictionary<BigInteger, int[]> countStatesAction = new Dictionary<BigInteger, int[]>();
        private Dictionary<BigInteger, double[]> qvalues;

        // exploration policy
        private IExplorationPolicy explorationPolicy;

        // discount factor
        private double discountFactor = 0.95;

        // learning rate
        private double learningRate = 0.25;


        /// <summary>
        /// Amount of possible states.
        /// </summary>
        /// 
        public BigInteger StatesCount
        {
            get { return states; }
        }

        /// <summary>
        /// Amount of possible actions.
        /// </summary>
        /// 
        public int ActionsCount
        {
            get { return actions; }
        }

        /// <summary>
        /// Exploration policy.
        /// </summary>
        /// 
        /// <remarks>Policy, which is used to select actions.</remarks>
        /// 
        public IExplorationPolicy ExplorationPolicy
        {
            get { return explorationPolicy; }
            set { explorationPolicy = value; }
        }

        /// <summary>
        /// Gets the number of states that have already been explored by the algorithm.
        /// </summary>
        /// 
        public int TriedStatesCount
        {
            get { return qvalues.Count; }
        }

        /// <summary>
        /// Learning rate, [0, 1].
        /// </summary>
        /// 
        /// <remarks>The value determines the amount of updates Q-function receives
        /// during learning. The greater the value, the more updates the function receives.
        /// The lower the value, the less updates it receives.</remarks>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                if (value < 0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("Argument should be between 0 and 1.");
                learningRate = value;
            }
        }

        /// <summary>
        /// Discount factor, [0, 1].
        /// </summary>
        /// 
        /// <remarks>Discount factor for the expected summary reward. The value serves as
        /// multiplier for the expected reward. So if the value is set to 1,
        /// then the expected summary reward is not discounted. If the value is getting
        /// smaller, then smaller amount of the expected reward is used for actions'
        /// estimates update.</remarks>
        /// 
        public double DiscountFactor
        {
            get { return discountFactor; }
            set
            {
                if (value < 0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("Discount factor should be between 0 and 1.");
                discountFactor = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteQLearning"/> class.
        /// </summary>
        /// 
        /// <param name="states">Amount of possible states.</param>
        /// <param name="actions">Amount of possible actions.</param>
        /// <param name="explorationPolicy">Exploration policy.</param>
        /// 
        /// <remarks>The <b>randomize</b> parameter specifies if initial action estimates should be randomized
        /// with small values or not. Randomization of action values may be useful, when greedy exploration
        /// policies are used. In this case randomization ensures that actions of the same type are not chosen always.</remarks>
        /// 
        public InfiniteQLearning(int states, int actions, IExplorationPolicy explorationPolicy)
        {
            this.states = states;
            this.actions = actions;
            this.explorationPolicy = explorationPolicy;

            // create Q-array
            qvalues = new Dictionary<BigInteger, double[]>();
        }

        /// <summary>
        /// Get next action from the specified state.
        /// </summary>
        /// 
        /// <param name="state">Current state to get an action for.</param>
        /// 
        /// <returns>Returns the action for the state.</returns>
        /// 
        /// <remarks>The method returns an action according to current
        /// <see cref="ExplorationPolicy">exploration policy</see>.</remarks>
        /// 
        public int GetAction(int state)
        {
            return explorationPolicy.ChooseAction(Q(state));
        }

        /// <summary>
        /// Update Q-function's value for the previous state-action pair.
        /// </summary>
        /// 
        /// <param name="previousState">Previous state.</param>
        /// <param name="action">Action, which leads from previous to the next state.</param>
        /// <param name="reward">Reward value, received by taking specified action from previous state.</param>
        /// <param name="nextState">Next state.</param>
        /// 
        public void UpdateState(int previousState, int action, double reward, int nextState)
        {
            if (!countStatesAction.ContainsKey(previousState))
            {
                countStatesAction[previousState] = new int[actions];
            }
            countStatesAction[previousState][action]++;
            // next state's action estimations
            double[] nextActionEstimations = Q(nextState);
            // find maximum expected summary reward from the next state
            double maxNextExpectedReward = nextActionEstimations[0];

            for (int i = 1; i < actions; i++)
            {
                if (nextActionEstimations[i] > maxNextExpectedReward)
                    maxNextExpectedReward = nextActionEstimations[i];
            }

            // previous state's action estimations
            double[] previousActionEstimations = Q(previousState);
            // update expexted summary reward of the previous state
            previousActionEstimations[action] = previousActionEstimations[action] * (1.0 - learningRate);
            previousActionEstimations[action] = previousActionEstimations[action] + (learningRate * (reward + discountFactor * maxNextExpectedReward));
        }

        private double[] Q(BigInteger nextState)
        {
            if (!qvalues.ContainsKey(nextState))
                qvalues[nextState] = new double[actions];
            return qvalues[nextState];
        }
    }
}