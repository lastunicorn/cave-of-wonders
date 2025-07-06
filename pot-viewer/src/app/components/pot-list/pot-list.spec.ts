import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PotList } from './pot-list';

describe('PotList', () => {
  let component: PotList;
  let fixture: ComponentFixture<PotList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PotList]
    })
      .compileComponents();

    fixture = TestBed.createComponent(PotList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
