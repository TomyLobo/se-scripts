public void AddGroup<T>(List<IMyTerminalBlock> list) 
{ 
    var secondary = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<T>(secondary); 
    list.AddRange(secondary); 
} 
void Main() 
{ 
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>(); 
    AddGroup<IMyBeacon>(blocks); 
    AddGroup<IMyRadioAntenna>(blocks);
    AddGroup<IMyProgrammableBlock>(blocks);
    
    IMyTerminalBlock NameBlock=null;
    //IMyTerminalBlock ProgramBlock=null;
    for (int i = 0; i < blocks.Count; i++)  
    {  
        if (blocks[i].CustomName.StartsWith("["))  
        {  
            NameBlock=blocks[i];
            UpdateText(  NameBlock,"");
            
        }
        //else if(blocks[i].CustomName.StartsWith("|*| ")){
        //ProgramBlock=blocks[i];
        //}		
    }  
    
    
    //an attempt to make the script restart itself... i ended up using a timer block to run the program then restart the timer until i find a better solution
    //~ if(ProgramBlock!=null){
    //~ var ProgramActionRun=ProgramBlock.GetActionWithName("Run");
    
    //~ if(ProgramActionRun!=null){
    
    //  UpdateText(  NameBlock,"Found Run Action");   
    //   ProgramBlock.GetActionWithName("Run").Apply(ProgramBlock); 
    //  UpdateText(  NameBlock,"Ran Run Action");   
    //~ }else{
    //~ UpdateText(  NameBlock,"Failed to Find Run Action");    
    //~ }
    //~ }else{
    //   UpdateText(  NameBlock,"Failed to Find Program Block");  
    //~ UpdateText(  NameBlock,"[ " +  blocks.Count + " ]"+blocks[0].DetailedInfo.ToString());  

    //~ }
    
    //end attempt to make the script restart
    
}


//added ability to append text to the end
void UpdateText( IMyTerminalBlock NameBlock ,string TextLabel)  
{  

    //if you the location to update faster than every second use this for loop	
    //   for (int i = 0; i <100; i++)  
    //{  
    // if(i%2 ==0){
    var BlockPosition=NameBlock.GetPosition();
    
    /////////this is only needed for the pretty way////////////
    double x = Math.Floor(BlockPosition.GetDim(0));  
    double y = Math.Floor(BlockPosition.GetDim(1));  
    double z = Math.Floor(BlockPosition.GetDim(2));  
    /////////////////////

    
    //  NameBlock.SetCustomName("[" +BlockPosition.ToString() + "] " + TextLabel+ " ");   // more efficient  and accurate ... less pretty
    NameBlock.SetCustomName("[X: " + x.ToString() + " Y: " + y.ToString() + " Z: " + z.ToString() + "] " + TextLabel+ " ");  //pretty not very efficient
    
    // }
    //}
    
}