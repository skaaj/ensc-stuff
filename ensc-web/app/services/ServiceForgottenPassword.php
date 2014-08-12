<?php
namespace Services;
require_once("app/PHPMailer/class.phpmailer.php");
class ServiceForgottenPassword{

	private static function generateUniqueKey(){
		return md5(uniqid(rand(), true));
	}

	public static function sendBackupEmail(\Data\DataProvider $dataProvider, $email){

		$destinataire = $email;
		$key = self::generateUniqueKey();
		$message = "<p>Bonjour, <br/> Pour réinitialiser votre mot de passe, <br/>
		veuillez vous rendre sur <a href=\"http://localhost/ensc-web/passwordbackup/".$email."/".$key."\">cette page</a></p>";
		$subject="Réinitialisez votre mot de passe";
		$valuesToInsert=array("user_email"=>$email, "sentkey"=>$key);
		$table="forgotten_pwd";
		if($dataProvider->insert($table, $valuesToInsert)){
			self::sendEmail($message, $email, $subject);

		}
	}

	public static function validateBackupUrl(\Data\DataProvider $dataProvider, $email, $key){
		$tables=array("forgotten_pwd");
		$colname="count";
		$columns=array("COUNT(*) AS ".$colname);
		$properties=array("sentkey"=>$key, "user_email"=>$email);
	    $data =$dataProvider->selectFieldsFromTableWithProperties( $tables,  $columns,  $properties, true, true, true, true);
		$validUrl = false;
		if($data[0]->$colname ==1){
			$validUrl=true;
		}
		return $validUrl;
	}


	public static function changePassword(\Data\DataProvider $dataProvider, $email, $password){
		$userProps = new \Entities\User(null, $email, null, null, null, null, null, null, null, $dataProvider);
		$dataUser = \Entities\User::findByProperties($dataProvider, $userProps, true, false, true);
		if(count($dataUser)>0){
			$user = $dataUser[0];
			$user->ignorePreviousChanges();
			$user->setPassword($password);
			$user->update();
			self::sendEmail("Your password has been changed successfully", $email, "password changed");
		}
	}

	public static function sendEmail($message, $email, $subject){
		$mail = new \PHPMailer(); // create a new object
			$mail->IsSMTP(); // enable SMTP
			$mail->CharSet="UTF-8";
			$mail->SMTPDebug = 2; // debugging: 1 = errors and messages, 2 = messages only
			$mail->SMTPAuth = true; // authentication enabled
			$mail->SMTPSecure = 'ssl'; // secure transfer enabled REQUIRED for GMail
			$mail->Host = "smtp.gmail.com";
			$mail->Port = 465; // or 587
			$mail->IsHTML(true);
			$mail->Username = "assistance.askit@gmail.com";
			$mail->Password = "askit2014";
			$mail->SetFrom("assistance.askit@gmail.com");
			$mail->Subject = $subject;
			$mail->Body = $message;
			$mail->AddAddress($email);

			if(!$mail->Send())
			{
			  echo "Mailer Error: " . $mail->ErrorInfo;
			}
			else
			{
			  echo "Message sent!";
			}
	}

}


?>