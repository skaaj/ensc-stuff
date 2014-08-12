<?php
namespace ValueObjects;


class RoleVO implements \Serializable{
	private $id;
	private $name;

	
	function __construct($id=null, $name=null){

		
		if($id!=null){ $this->id=$id; }
		
		if($name!=null){ $this->name=$name; }
	}
	
	public function getName(){
		return $this->name;
	}
	public function getId(){
		return $this->id;
	}
	public function serialize() {
 	return serialize(
            array(
                'id' => $this->id,
                'name' => $this->name,
            )
        );
	}

	public function unserialize($data) {
		$data = unserialize($data);
        $this->id = $data['id'];
        $this->name = $data['name'];
	}
	
}



?>