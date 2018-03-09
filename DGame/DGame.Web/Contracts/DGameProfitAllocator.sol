pragma solidity ^0.4.18;

contract DGameProfitAllocator {
    uint256 constant ADVERTISE_TAX = 20000000000000000;
    address public owner;
    mapping(address => uint256) funds;
    mapping(string => address) names;
    mapping(string => address[]) views;
    mapping(address => uint256) allViewsByProfile;
    
    function DGameProfitAllocator() public payable {
        owner = msg.sender;
    }
    
    function addGame(string name, address addr) public {
        require(msg.sender == owner);
        names[name] = addr;
    }
    
    function addViewToGame(string name, address viewer) public {
        require(msg.sender == owner);
        views[name].push(viewer);
        
        address gameCreator = names[name];
        allViewsByProfile[gameCreator] += 1;
    }
    
    function getFunds(address addr) view public returns(uint256) {
        return funds[addr];
    }
    
    function transferFunds() public payable {
        require(allViewsByProfile[msg.sender] > 0);
        require(this.balance > 0);
        
        uint256 amount = 115000000000000 * allViewsByProfile[msg.sender];
        allViewsByProfile[msg.sender] = 0;
        
        msg.sender.transfer(amount);
    }
    
    function transfer() public payable {
        require(msg.value >= ADVERTISE_TAX);
        funds[msg.sender] += msg.value;
    }
    
    function getBalance() public view returns(uint) {
        return this.balance;
    }
    
    function getViews(address addr) public view returns(uint) {
        return allViewsByProfile[addr];
    }
}