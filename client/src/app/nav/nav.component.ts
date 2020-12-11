import { Component, OnInit } from '@angular/core';
import { error } from 'protractor';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};  

  constructor(public accountService : AccountService) { 
  }

  ngOnInit(): void {
  }

  logIn() {
    this.accountService.login(this.model).subscribe(response=>{      
      console.log(response);
    },error=>{
      console.log(error);
    });
  }

  logout() {
    this.accountService.logout();
  } 
}


//loggedIn: boolean = false;
// logIn() {
  //   this.accountService.login(this.model).subscribe(response=>{      
  //     this.loggedIn = true;
  //     console.log(response);
  //   },error=>{
  //     console.log(error);
  //   });
  // }

  // logout() {
  //   this.accountService.logout();
  //   this.loggedIn = false;
  // }

  // getCurrentUser() { 
  //   this.accountService.currentUser$.subscribe(user => {
  //     this.loggedIn = !! user;
  //   },error => {
  //     console.log(error);
  //   });
  // }