import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import LoginModel from "../models/login.model";
import AuthResponseModel from "../models/authResponse.model";
import {environment} from "../../../environments/environment";
import RegisterModel from "../models/register";
import UserModel from "../models/user.model";
import {BehaviorSubject} from "rxjs";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  user?: UserModel

  constructor(private httpClient: HttpClient, private router: Router) {
    const token = localStorage.getItem('jwtToken');
    const userName = localStorage.getItem('userName');
    if (token && userName) {
      // You might want to verify the token's validity here before considering the user as authenticated
      // For example, check if the token is expired
      // If the token is valid, log the user in
      this.login(token, userName);
    }
  }

  authenticate(loginModel: LoginModel) {
    return this.httpClient.post<AuthResponseModel>(`${environment.apiUrl}/api/users/Login`, loginModel);
  }

  login(token: string, userName: string) {
    localStorage.setItem('jwtToken', token);
    localStorage.setItem('userName', userName);
    this.user = {userName: userName};
    this.isAuthenticatedSubject.next(true);
  }

  register(registerModel: RegisterModel) {
    return this.httpClient.post(`${environment.apiUrl}/api/users/Register`, registerModel);
  }

  logout() {
    this.user = undefined;
    localStorage.removeItem('jwtToken');
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/']);
  }
}
