# CrowdSimulator
Aims to simulate crowd movements in shopping malls using Unity by adding on to the NavMesh functionalities provided
![Screenshot](/Screenshot.PNG)

## Currently achieved
* Agents changing their obstacle avoidance radius based on immediate surroundings
* Crude heatmap cells that change colour based on density
* Extremely crude lift that just teleports people from one floor to another without any consideration
* Overhead text over each person to show their current goal

## To-do
* Improve lift
* Add stairs and escalators
* Add toilets and nursing rooms
* Add shop types (FnB, retail, services, etc)
* Add shop popularity
* Add time spent in shops
* Allow spontaneously changing goals based on what people see
* Allow walking around with no real goal
* Add more elements into decision making (such as time of day, time spent in mall, etc)
* Add people who travel in groups
* Further generalize everything to accomodate for real models in the future
* Improve distribution of heatmap cells (should not include walls)
* Improve movements in crowded spaces (currently people do not make way for one another) and sparse places (currently people still keep hugging the walls)
* Improve performance for larger speedup scales (currently only performs well around 10x or less)
* Improve performance for larger spawn rates (with 4 entrances and 1 lift it currently does not perform well for >= 1 person per second in real time)
* Interactive cameras for easier viewing when running the simulation
