import React, { useEffect, useState } from "react";
import "./HomePage.css";
import ToDoListServices from "../services/ToDoListServices";
import TextField from "@material-ui/core/TextField";
import Pagination from "@material-ui/lab/Pagination";
import ThumbUpIcon from "@material-ui/icons/ThumbUp";
import Button from "@material-ui/core/Button";
import Radio from "@material-ui/core/Radio";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import RadioGroup from "@material-ui/core/RadioGroup";
import Snackbar from "@material-ui/core/Snackbar";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import dayjs from "dayjs";
import Popover from "@material-ui/core/Popover";
import Checkbox from "@material-ui/core/Checkbox";
import Typography from "@material-ui/core/Typography";
import Box from "@material-ui/core/Box";

const toDoListServices = new ToDoListServices();
//
export default function HomePage() {
  const [NoteID, setNoteID] = useState("");
  const [NotesFlag, setNotesFlag] = useState(false);
  const [EditFlag, setEditFlag] = useState(false);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [PopData, setPopData] = useState({
    Note: "",
  });
  const [Data, setData] = useState({
    Notes: "",
    ScheduleDate: "",
    ScheduleTime: "",
    Sortvalue: "Desc",
    Monday: false,
    Tuesday: false,
    Wednesday: false,
    Thursday: false,
    Friday: false,
    Saturday: false,
    Sunday: false,
  });
  const [openSnackBar, setOpenSnackBar] = React.useState(false);
  const [SnackMessage, setSnackMessage] = useState("");
  const [PageNumber, setPageNumber] = useState(1);
  const [NumberOfRecordPerPage, setNumberOfRecordPerPage] = useState(4);
  const [TotalPages, setTotalPages] = useState(0);
  const [NoteData, setNoteData] = useState([]);

  useEffect(() => {
    console.log("Use Effect Calling ....");
    GetNotes(PageNumber, Data.Sortvalue);
  }, []);

  const GetNotes = (PageNumber, Sort) => {
    console.log("Get Notes Calling ....");
    let data = {
      pageNumber: PageNumber,
      numberOfRecordPerPage: NumberOfRecordPerPage,
      sortBy: Sort,
    };
    toDoListServices
      .GetNote(data)
      .then((data) => {
        // debugger;
        console.log("Data : ", data);
        if (data.data.data === null && PageNumber - 1 > 0) {
          GetNotes(PageNumber - 1, Sort);
        }

        if (data.data.data !== null) {
          setNoteData(data.data.data);
          setTotalPages(data.data.totalPages);
          setPageNumber(data.data.currentPage);
        } else {
          setNoteData(null);
          setTotalPages(0);
          setPageNumber(1);
        }
      })
      .catch((error) => {
        console.log("Error : ", error);
        setSnackMessage("Something Went Wrong");
        setOpenSnackBar(true);
      });
  };

  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }

    setOpenSnackBar(false);
  };

  const handleSortChange = async (event) => {
    console.log("Sort : ", event.target.value);
    await setData({ ...Data, Sortvalue: event.target.value });
    await GetNotes(PageNumber, event.target.value);
  };

  const handlePaging = (event, value) => {
    console.log("Page Number : ", value);
    setPageNumber(value);
    GetNotes(value, Data.Sortvalue);
  };

  const handleDone = () => {
    setNotesFlag(false);
    if (Data.Notes === "") {
      setSnackMessage("Dude Please, Enter Your Note");
      setOpenSnackBar(true);
      setNotesFlag(true);
      return;
    }
    // var formattedDate = format(ScheduleDate, 'dd-MMMM-yyyy H:mma')
    // debugger;
    if (EditFlag === false) {
      const data = {
        note: Data.Notes,
        scheduleDate: Data.ScheduleDate,
        scheduleTime: Data.ScheduleTime,
        monday: Data.Monday,
        tuesday: Data.Tuesday,
        wednesday: Data.Wednesday,
        thursday: Data.Thursday,
        friday: Data.Friday,
        saturday: Data.Saturday,
        sunday: Data.Sunday,
      };
      // debugger;
      toDoListServices
        .InsertNote(data)
        .then((data) => {
          // debugger;
          console.log("Data : ", data);
          setSnackMessage(data.data.message);
          setOpenSnackBar(true);
          GetNotes(PageNumber, Data.Sortvalue);
        })
        .catch((error) => {
          console.log("Error : ", error);
          setSnackMessage("Something Went Wrong");
          setOpenSnackBar(true);
          GetNotes(PageNumber, Data.Sortvalue);
        });
    } else {
      const data = {
        id: NoteID,
        note: Data.Notes,
        scheduleDate: Data.ScheduleDate,
        scheduleTime: Data.ScheduleTime,
        monday: Data.Monday,
        tuesday: Data.Tuesday,
        wednesday: Data.Wednesday,
        thursday: Data.Thursday,
        friday: Data.Friday,
        saturday: Data.Saturday,
        sunday: Data.Sunday,
      };

      toDoListServices
        .UpdateNote(data)
        .then((data) => {
          console.log("Data : ", data);
          setSnackMessage(data.data.message);
          setOpenSnackBar(true);
          GetNotes(PageNumber, Data.Sortvalue);
          setNotes("");
          setScheduleDate(Date().toLocaleString());
        })
        .catch((error) => {
          console.log("Error : ", error);
          setSnackMessage("Something Went Wrong");
          setOpenSnackBar(true);
          GetNotes(PageNumber, Data.Sortvalue);
        });
    }
    // console.log(data);
    setEditFlag(false);
    handleClear();
  };

  const handleEditNote = (id) => {
    // debugger;
    console.log("handle Edit Note id : ", id);
    toDoListServices
      .GetNoteById(id)
      .then((data) => {
        // debugger;
        console.log("Data : ", data);
        if (data.data.data !== null) {
          setNoteID(id);
          setEditFlag(true);
          setData({
            ...Data,
            Notes: data.data.data.note,
            ScheduleDate: data.data.data.scheduleDate,
            ScheduleTime: data.data.data.scheduleTime,
            Monday: data.data.data.monday,
            Tuesday: data.data.data.tuesday,
            Wednesday: data.data.data.wednesday,
            Thursday: data.data.data.thursday,
            Friday: data.data.data.friday,
            Saturday: data.data.data.saturday,
            Sunday: data.data.data.sunday,
          });
          setSnackMessage(data.data.message);
          setOpenSnackBar(true);
        }
      })
      .catch((error) => {
        console.log("Error : ", error);
        setSnackMessage("Something Went Wrong");
        setOpenSnackBar(true);
      });
  };

  const handleDeleteNote = (id) => {
    console.log("handle Delete Note id : ", id);
    toDoListServices
      .DeleteNote(id)
      .then((data) => {
        // debugger;
        console.log("Data : ", data);
        setSnackMessage(data.data.message);
        setOpenSnackBar(true);
        GetNotes(PageNumber, Data.Sortvalue);
      })
      .catch((error) => {
        console.log("Error : ", error);
        setSnackMessage("Something Went Wrong");
        setOpenSnackBar(true);
      });
  };

  const handleClear = () => {
    setData({
      Notes: "",
      ScheduleDate: "",
      ScheduleTime: "",
      Sortvalue: "Desc",
      Monday: false,
      Tuesday: false,
      Wednesday: false,
      Thursday: false,
      Friday: false,
      Saturday: false,
      Sunday: false,
    });
  };

  const handlePopoverOpen = (event, data) => {
    console.log("handlePopoverOpen Calling ... Data : ", data);
    setAnchorEl(event.currentTarget);
    setPopData({ ...PopData, Note: data });
  };

  const handlePopoverClose = () => {
    console.log("handlePopoverClose Calling ...");
    setAnchorEl(null);
    setPopData({ ...PopData, Note: "" });
  };

  return (
    <div className="Container">
      <div className="Sub-Container">
        <div className="Body1">
          <TextField
            className="Note"
            error={NotesFlag}
            autoComplete="Off"
            size="small"
            fullWidth
            label="Notes"
            variant="outlined"
            value={Data.Notes}
            onChange={(e) => {
              setData({ ...Data, Notes: e.target.value }),
                console.log("Notes : ", e.target.value);
            }}
          />
          <div className="Date">
            <div>Schedule Date</div>
            <TextField
              className="Schedule-Date"
              autoComplete="Off"
              size="small"
              type="date"
              variant="outlined"
              value={Data.ScheduleDate}
              onChange={(e) => {
                setData({
                  ...Data,
                  ScheduleDate: e.target.value,
                }),
                  console.log(
                    "Schedule Date :",
                    dayjs(e.target.value).format("DD/MM/YYYY")
                  );
              }}
            />
            <div>Schedule Time</div>
            <TextField
              className="Schedule-Time"
              autoComplete="Off"
              size="small"
              type="time"
              variant="outlined"
              value={Data.ScheduleTime}
              onChange={(e) => {
                setData({ ...Data, ScheduleTime: e.target.value }),
                  console.log("Schedule Time : ", e.target.value);
              }}
            />
          </div>
          <div className="Done">
            <RadioGroup
              className="RadioGroup"
              value={Data.Sortvalue}
              onChange={handleSortChange}
            >
              <FormControlLabel value="Asc" control={<Radio />} label="Asc" />
              <FormControlLabel value="Desc" control={<Radio />} label="Desc" />
            </RadioGroup>
            <div style={{ height: 60 }}>
              <div style={{ width: 350, height: 22, display: "flex" }}>
                <div style={{ flex: 1 }}>Mon</div>
                <div style={{ flex: 1 }}>Tue</div>
                <div style={{ flex: 1 }}>Web</div>
                <div style={{ flex: 1 }}>Thu</div>
                <div style={{ flex: 1 }}>Fri</div>
                <div style={{ flex: 1 }}>Sat</div>
                <div style={{ flex: 1 }}>Sun</div>
              </div>
              <div style={{ width: 350, height: 22, display: "flex" }}>
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Monday}
                  onChange={(e) => {
                    setData({ ...Data, Monday: e.target.checked });
                  }}
                />
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Tuesday}
                  onChange={(e) => {
                    setData({ ...Data, Tuesday: e.target.checked });
                  }}
                />
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Wednesday}
                  onChange={(e) => {
                    setData({ ...Data, Wednesday: e.target.checked });
                  }}
                />
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Thursday}
                  onChange={(e) => {
                    setData({ ...Data, Thursday: e.target.checked });
                  }}
                />
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Friday}
                  onChange={(e) => {
                    setData({ ...Data, Friday: e.target.checked });
                  }}
                />
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Saturday}
                  onChange={(e) => {
                    setData({ ...Data, Saturday: e.target.checked });
                  }}
                />
                <Checkbox
                  style={{ flex: 1 }}
                  checked={Data.Sunday}
                  onChange={(e) => {
                    setData({ ...Data, Sunday: e.target.checked });
                  }}
                />
              </div>
            </div>
            <div className="Button">
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleDone()}
              >
                <ThumbUpIcon />
                &nbsp; Done
              </Button>
            </div>
          </div>
        </div>
        <div className="Body2">
          <div
            className="Data-flex"
            style={{ margin: "5px 0 0 0", color: "blue" }}
          >
            <div className="NoteID" style={{ flex: 1 }}>
              Note ID
            </div>
            <div className="Notes" style={{ width: 300, margin: "0 30px" }}>
              Notes
            </div>
            <div className="Days" style={{ flex: 3 }}>
              Days
            </div>
            <div className="Schedule-DateTime" style={{ flex: 2.5 }}>
              Schedule Date
            </div>
            <div className="Schedule-DateTime" style={{ flex: 2.5 }}>
              Schedule Time
            </div>
            <div className="Operation" style={{ flex: 2.5 }}>
              Setting
            </div>
          </div>
          {Array.isArray(NoteData) &&
            NoteData.map(function (data, index) {
              return (
                <div className="Data-flex" key={index}>
                  <div className="NoteID" style={{ flex: 1 }}>
                    {data.noteId}
                  </div>
                  <div
                    className="Notes"
                    style={{
                      // flex: 6,
                      width: 300,
                      margin: "0 30px",
                      textOverflow: "ellipsis",
                      overflow: "hidden",
                    }}
                    onMouseEnter={(e) => {
                      handlePopoverOpen(e, data.note);
                    }}
                    onMouseLeave={handlePopoverClose}
                  >
                    {data.note}
                  </div>
                  <div className="Days">
                    <Checkbox style={{ flex: 1 }} checked={data.monday} />
                    <Checkbox style={{ flex: 1 }} checked={data.tuesday} />
                    <Checkbox style={{ flex: 1 }} checked={data.wednesday} />
                    <Checkbox style={{ flex: 1 }} checked={data.thursday} />
                    <Checkbox style={{ flex: 1 }} checked={data.friday} />
                    <Checkbox style={{ flex: 1 }} checked={data.saturday} />
                    <Checkbox style={{ flex: 1 }} checked={data.sunday} />
                  </div>
                  <div className="Schedule-DateTime" style={{ flex: 2.5 }}>
                    {data.scheduleDate === null ? <>NA</> : data.scheduleDate}
                  </div>
                  <div className="Schedule-DateTime" style={{ flex: 2.5 }}>
                    {data.scheduleTime === null ? <>NA</> : data.scheduleTime}
                  </div>
                  <div className="Operation" style={{ flex: 2.5 }}>
                    <Button
                      variant="outlined"
                      size="small"
                      style={{ margin: "0 5px 0 0" }}
                      onClick={() => {
                        handleEditNote(data.noteId);
                      }}
                    >
                      <EditIcon />
                    </Button>
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={() => {
                        handleDeleteNote(data.noteId);
                      }}
                    >
                      <DeleteIcon />
                    </Button>
                  </div>
                </div>
              );
            })}
        </div>
      </div>
      <div className="Pagination">
        <Pagination
          count={TotalPages}
          page={PageNumber}
          onChange={handlePaging}
          variant="outlined"
          shape="rounded"
        />
      </div>
      <Popover
        id="mouse-over-popover"
        style={{
          pointerEvents: "none",
        }}
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "center",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "left",
        }}
        onClose={handlePopoverClose}
        disableRestoreFocus
      >
        <Box p={2} style={{ backgroundColor: "black", color: "white" }}>
          <Typography>{PopData.Note}</Typography>
        </Box>
      </Popover>
      <Snackbar
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "left",
        }}
        open={openSnackBar}
        autoHideDuration={3000}
        onClose={handleClose}
        message={SnackMessage}
        action={
          <React.Fragment>
            <Button color="secondary" size="small" onClick={handleClose}>
              UNDO
            </Button>
            <IconButton
              size="small"
              aria-label="close"
              color="inherit"
              onClick={handleClose}
            >
              <CloseIcon fontSize="small" />
            </IconButton>
          </React.Fragment>
        }
      />
    </div>
  );
}
