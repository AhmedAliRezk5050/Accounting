import {Component} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AlertService} from "../../../shared/services/alert.service";
import {FormValidators} from "../../../shared/validators/FormValidators";
import {AuthService} from "../../services/auth.service";
import {ModalService} from "../../../shared/services/modal.service";

@Component({
  selector: 'app-register-modal',
  templateUrl: './register-modal.component.html',
  styleUrls: ['./register-modal.component.scss']
})
export class RegisterModalComponent {


  constructor(
    public alertService: AlertService,
    private authService: AuthService,
    private modalService: ModalService
  ) {
  }

  userName = new FormControl('', [
    Validators.required,
    // Validators.minLength(3)
  ]);

  password = new FormControl(
    '',
    [
      Validators.required,
      // Validators.pattern(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/gm)
    ]
  );

  confirmPassword = new FormControl('');


  registerForm = new FormGroup({
    userName: this.userName,
    password: this.password,
    confirmPassword: this.confirmPassword
  },[FormValidators.matchPassword('password', 'confirmPassword')]);

  register() {
    this.alertService.alertShown = true;
    this.alertService.alertMsg = 'Please wait, your account is being created.';
    this.alertService.alertColor = 'blue';

    this.authService
      .register({userName: this.userName.value!, password: this.password.value!})
      .subscribe({
        next: res => {
          this.alertService.alertColor = 'green';
          this.alertService.alertMsg = 'Account created successfully.';

          setTimeout(() => {
            this.alertService.alertShown = false;
            this.modalService.toggleModal('register', false);
          }, 1000);
        },
        error: _ => {
          this.alertService.alertColor = 'red';
          this.alertService.alertMsg = 'Registering failed.';
        }
      });
  }
}
