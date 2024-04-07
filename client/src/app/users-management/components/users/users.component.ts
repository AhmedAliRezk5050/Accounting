import {Component, OnInit} from '@angular/core';
import {UserDetailsModel} from "../../models/user-details.model";
import {UsersManagementService} from "../../services/users-management.service";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit{
  users: UserDetailsModel[] = [];

  constructor(private usersManagementService: UsersManagementService) {}

  ngOnInit(): void {
    this.usersManagementService.getUsers().subscribe(data => {
      this.users = data;
    });
  }
}
